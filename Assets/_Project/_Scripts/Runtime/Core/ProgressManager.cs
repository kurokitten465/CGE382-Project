using System;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using PingPingProduction.ProjectAnomaly.Interaction;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class ProgressManager : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] byte _maxAnomalyFounded = 6;

        [Header("Depemdencies")]
        [SerializeField] RoomManager _roomManager;
        [SerializeField] HallwayConfig _bossHallway;
        [SerializeField] HallwayConfig _defaultHallway;
        [SerializeField] TMP_Text _progressText;
        byte _currentProgress = 0;

        /*         [Header("Debugger")]
                [SerializeField] HallwayConfig _hallway_0;
                [SerializeField] HallwayConfig _hallway_1;
                [SerializeField] HallwayConfig _hallway_2;
                [SerializeField] HallwayConfig _hallway_3;
                [SerializeField] HallwayConfig _hallway_4;
                [SerializeField] HallwayConfig _hallway_5;
                [SerializeField] HallwayConfig _hallway_6;
                [SerializeField] HallwayConfig _hallway_7;
                [SerializeField] HallwayConfig _hallway_8; */

        public static Action<ElevatorButtonTrigger> OnElevatorButtonTriggered;
        public static bool IsResolving = false;

        public static byte AnomalyFounded { get; private set; } = 0;

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
            _currentProgress = 0;
            _progressText.text = _currentProgress.ToString();
            await GameManager.Instance.FadingCanvas.DOFade(0f, 3f).From(1f, true).AsyncWaitForCompletion().AsUniTask();
            GameManager.Instance.Pause();
            IsResolving = false;
        }

        async UniTaskVoid OnHallwaySequence(bool isWin, ElevatorButtonTrigger buttonTrigger) {
            if (!isWin) {
                AnomalyFounded = 0;
                _currentProgress = 0;
                _progressText.text = _currentProgress.ToString();
                GameManager.Instance.IsBossRoom = false;
                await _roomManager.GenerateAsync(buttonTrigger, true);
                IsResolving = false;
                Debug.Log($"Lost! Progrees: {AnomalyFounded}/{_maxAnomalyFounded}");
            }
            else {
                if (_roomManager.CurrentHallway.IsAnomaly) {
                    AnomalyFounded++;
                    _currentProgress++;
                    _progressText.text = _currentProgress.ToString();
                    GameManager.Instance.UpdateAnomalyFlag(_roomManager.CurrentHallway);
                    Debug.Log($"Win! Progrees: {AnomalyFounded}/{_maxAnomalyFounded}");
                }

                if (AnomalyFounded < _maxAnomalyFounded) {
                    await _roomManager.GenerateAsync(buttonTrigger);

                    IsResolving = false;
                }
                else {
                    if (!GameManager.Instance.IsBossRoom) {
                        await _roomManager.GenerateAsync(buttonTrigger, _bossHallway);
                        IsResolving = false;
                        GameManager.Instance.IsBossRoom = true;
                    }
                    else {
                        GameManager.Instance.IsBossRoom = false;
                        _roomManager.GenerateAsync(buttonTrigger, _defaultHallway).Forget();

                        await Task.Delay(2000);

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
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        float sliderValue = 0.6f;

        void OnGUI() {
            GUI.Label(new Rect(10, 10, 600, 30), $"Current Hallway: {_roomManager.CurrentHallway.HallwayPrefab.name}");
            GUI.Label(new Rect(10, 40, 600, 30), $"Current Anomaly Chance: {sliderValue}");
            sliderValue = GUI.HorizontalSlider(new Rect(10, 80, 300, 50), sliderValue, 0.1f, 1f);

            _roomManager.SetAnomalyChance(sliderValue);

            /*             if (GUI.Button(new Rect(10, 70, 200, 40), "Default Room"))
                            _roomManager.DebugGenerate(_defaultHallway);

                        if (GUI.Button(new Rect(10, 110, 200, 40), "Boss Room"))
                            _roomManager.DebugGenerate(_bossHallway);

                        if (GUI.Button(new Rect(10, 110, 200, 40), "Anomaly Room 1"))
                            _roomManager.DebugGenerate(_bossHallway);

                        if (GUI.Button(new Rect(10, 110, 200, 40), "Anomaly Room 2"))
                            _roomManager.DebugGenerate(_bossHallway);

                        if (GUI.Button(new Rect(10, 110, 200, 40), "Anomaly Room 3"))
                            _roomManager.DebugGenerate(_bossHallway);

                        if (GUI.Button(new Rect(10, 110, 200, 40), "Room 4"))
                            _roomManager.DebugGenerate(_bossHallway);

                        if (GUI.Button(new Rect(10, 110, 200, 40), "Room 4"))
                            _roomManager.DebugGenerate(_bossHallway);

                        if (GUI.Button(new Rect(10, 110, 200, 40), "Room 4"))
                            _roomManager.DebugGenerate(_bossHallway);

                        if (GUI.Button(new Rect(10, 110, 200, 40), "Room 4"))
                            _roomManager.DebugGenerate(_bossHallway);

                        if (GUI.Button(new Rect(10, 110, 200, 40), "Room 4"))
                            _roomManager.DebugGenerate(_bossHallway); */
        }
#endif
    }
}
