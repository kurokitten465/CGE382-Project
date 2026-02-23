using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PingPingProduction.ProjectAnomaly.Interaction;
using PingPingProduction.ProjectAnomaly.Player;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class RoomManager : MonoBehaviour, IPauseContext {
        [Header("Settings")]
        [SerializeField] Transform _hallwaySpawner;
        [SerializeField] Transform _hallwayDestroyer;

        [Header("Animation & Transition")]
        [SerializeField] string _elevatorOpeningAnimName;
        [SerializeField] string _elevatorClosingAnimName;
        [SerializeField] float _elevatorUpDuration;
        [SerializeField] Animator _yuukiElevatorAnimator;
        [SerializeField] Animator _hinaElevatorAnimator;

        [Header("Dependencies")]
        [SerializeField] HallwayConfig _initializeConfig;

        // Members
        GameObject _currentHallway;
        GameObject _oldHallway;

        GameManager _gameManager;
        CancellationTokenSource _cts;

        void Awake() {
            _cts = new();
        }

        void Start() {
            _gameManager = GameManager.Instance;
            OnGameStarted().Forget();
        }

        void OnDestroy() {
            _cts.Cancel();
        }

        async UniTaskVoid OnGameStarted() {
            _gameManager.Pause(this);

            _currentHallway = Instantiate(_initializeConfig.HallwayPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity);

            await _currentHallway.transform
                    .DOMoveY(0f, _elevatorUpDuration / 2f)
                    .AsyncWaitForCompletion()
                    .AsUniTask()
                    .AttachExternalCancellation(_cts.Token);

            _yuukiElevatorAnimator.Play(_elevatorOpeningAnimName);
            _hinaElevatorAnimator.Play(_elevatorOpeningAnimName);

            await UniTask.Delay(2500);
            _gameManager.Pause(this);
        }

        public void OnLiftTriggered(ElevatorTrigger trigger, PlayerController player) {
            _gameManager.SetGameState(GameState.Resolving);
            OnLoadNextHallway(trigger).Forget();
        }

        async UniTaskVoid OnLoadNextHallway(ElevatorTrigger trigger) {
            trigger.ElevatorCam.Priority = 100;

            await UniTask.Delay(1000);

            _yuukiElevatorAnimator.Play(_elevatorClosingAnimName);
            _hinaElevatorAnimator.Play(_elevatorClosingAnimName);

            _gameManager.Pause(this);
            await UniTask.Delay(2500);

            trigger.ElevatorCam.Priority = -100;
        }
    }
}

