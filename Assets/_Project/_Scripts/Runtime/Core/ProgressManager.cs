using System;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using PingPingProduction.ProjectAnomaly.Interaction;
using System.Threading.Tasks;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class ProgressManager : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] byte _maxAnomalyFounded = 6;

        [Header("Depemdencies")]
        [SerializeField] RoomManager _roomManager;
        [SerializeField] GameObject _tmp;

        public static Action<ElevatorButtonTrigger> OnElevatorButtonTriggered;
        public static bool IsResolving = false;

        byte _anomalyFounded = 0;

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
            _anomalyFounded = 0;
            await GameManager.Instance.FadingCanvas.DOFade(0f, 3f).From(1f, true).AsyncWaitForCompletion().AsUniTask();
            GameManager.Instance.Pause();
            IsResolving = false;
        }

        async UniTaskVoid OnHallwaySequence(bool isWin, ElevatorButtonTrigger buttonTrigger) {
            if (!isWin) {
                await _roomManager.GenerateAsync(buttonTrigger, true);
                IsResolving = false;
                _anomalyFounded = 0;
                Debug.Log($"Lost! Progrees: {_anomalyFounded}/{_maxAnomalyFounded}");
            }
            else {
                await _roomManager.GenerateAsync(buttonTrigger);
                IsResolving = false;
                if (!_roomManager.PreviousHallway.IsAnomaly) return;

                _anomalyFounded++;
                Debug.Log($"Win! Progrees: {_anomalyFounded}/{_maxAnomalyFounded}");

                if (_anomalyFounded != _maxAnomalyFounded) return;
                _tmp.SetActive(true);
            }
        }

        void OnGUI() {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            GUI.Label(new Rect(10, 10, 300, 20), $"Current Hallway: {_roomManager.CurrentHallway.HallwayPrefab.name}");
            GUI.Label(new Rect(10, 30, 300, 20), $"Progress: {_anomalyFounded}/{_maxAnomalyFounded}");
#endif
        }
    }
}
