using System;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using PingPingProduction.ProjectAnomaly.Interaction;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Linq;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class ProgressManager : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] byte _maxAnomalyFounded = 6;

        [Header("Depemdencies")]
        [SerializeField] RoomManager _roomManager;

        public static Action<ElevatorButtonTrigger> OnElevatorButtonTriggered;
        public static bool IsResolving = false;

        public static byte AnomalyFounded {get; private set; } = 0;

        void Start() {
            IsResolving = true;
            OnGameStarted().Forget();
        }

        void OnEnable() {
            OnElevatorButtonTriggered += OnElevatorTriggerd;
        }

        void OnDisable() {
            OnElevatorButtonTriggered -= OnElevatorTriggerd;
        }

        void OnElevatorTriggerd(ElevatorButtonTrigger elevatorButton) {
            var isWin = CheckCodition(elevatorButton.ElevatorDirection, _roomManager.CurrentHallway);
            IsResolving = true;
            OnHallwaySequence(isWin, elevatorButton).Forget();
        }

        bool CheckCodition(ElevatorButtonDirection elevatorButton, HallwayConfig hallwayConfig) {
            return hallwayConfig.IsAnomaly == (elevatorButton == ElevatorButtonDirection.Upward);
        }

        async UniTask OnGameStarted() {
            _roomManager.Generate(0);
            AnomalyFounded = 0;
            await GameManager.Instance.FadingCanvas.DOFade(0f, 3f).From(1f, true).AsyncWaitForCompletion().AsUniTask();
            GameManager.Instance.Pause();
            IsResolving = false;
        }

        async UniTaskVoid OnHallwaySequence(bool isWin, ElevatorButtonTrigger buttonTrigger) {
            if (!isWin) {
                AnomalyFounded = 0;
                await _roomManager.GenerateAsync(buttonTrigger, true);
                IsResolving = false;
                Debug.Log($"Lost! Progrees: {AnomalyFounded}/{_maxAnomalyFounded}");
            }
            else {
                if (_roomManager.CurrentHallway.IsAnomaly) {
                    AnomalyFounded++;
                    GameManager.Instance.UpdateAnomalyFlag(_roomManager.CurrentHallway);
                    Debug.Log($"Win! Progrees: {AnomalyFounded}/{_maxAnomalyFounded}");
                }

                if (AnomalyFounded != _maxAnomalyFounded) {
                    await _roomManager.GenerateAsync(buttonTrigger);

                    IsResolving = false;
                }
                else {
                    _roomManager.GenerateAsync(buttonTrigger, true).Forget();
                    await GameManager.Instance.FadingCanvas.DOFade(1f, 2f).From(0f, true).AsyncWaitForCompletion().AsUniTask();
                    await GameManager.Instance.WinText.DOFade(1f, 1f).From(0f, true).AsyncWaitForCompletion().AsUniTask();

                    await Task.Delay(8000);

                    await GameManager.Instance.WinText.DOFade(0f, 1f).From(1f, true).AsyncWaitForCompletion().AsUniTask();

                    GameManager.Instance.UpdateAnomalyFlag(GameManager.Instance.CardCollectionKeys.Last().Key);

                    IsResolving = false;

                    OnLoading().Forget();
                }
            }
        }

        public async UniTaskVoid OnLoading() {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("scene_main_menu");

            // Stop the scene from activating immediately
            asyncLoad.allowSceneActivation = false;

            // Wait until the load is 90% complete (Unity's internal loading stops at 0.9)
            while (asyncLoad.progress < 0.9f) {
                // Update UI, etc.
                await UniTask.Yield();
            }

            // Now, allow the scene to activate (Awake/Start methods will run now, which can still cause a small hitch)
            asyncLoad.allowSceneActivation = true;

            // The rest of the while loop handles the final activation
            while (!asyncLoad.isDone) {
                await UniTask.Yield();
            }
        }

        void OnGUI() {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            GUI.Label(new Rect(10, 10, 400, 20), $"Current Hallway: {_roomManager.CurrentHallway.HallwayPrefab.name}");
            GUI.Label(new Rect(10, 30, 400, 20), $"Progress: {AnomalyFounded}/{_maxAnomalyFounded}");
#endif
        }
    }
}
