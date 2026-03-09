using UnityEngine;
using PingPingProduction.ProjectAnomaly.Interaction;
using PingPingProduction.ProjectAnomaly.Core;
using TMPro;

namespace PingPingProduction.ProjectAnomaly.Interaction {
    public class ElevatorButtonTrigger : MonoBehaviour, IInteractable {
        [Header("Settings")]
        [field: SerializeField] public ElevatorButtonDirection ElevatorDirection { get; private set; }

        [Header("Dependencies")]
        [SerializeField] ElevatorTrigger _elevatorTrigger;
        [SerializeField] Animator _elevatorBtnAnimator;
        [SerializeField] GameObject _WTMP;
        [SerializeField] Vector3 _offsets;

        GameObject _currentWTMP;

        public ElevatorTrigger ElevatorTrigger => _elevatorTrigger;

        bool _isPlayerInside = false;

        void OnEnable() {
            _elevatorTrigger.OnLiftTriggered += OnPlayerFullyInside;
        }

        void OnDisable() {
            _elevatorTrigger.OnLiftTriggered -= OnPlayerFullyInside;
        }

        public void OnPlayerFullyInside(bool isInside) => _isPlayerInside = isInside;

        public void Interact() {
            if (!_isPlayerInside || ProgressManager.IsResolving) return;
            ProgressManager.OnElevatorButtonTriggered?.Invoke(this);
        }

        public void OnPointedAt() {
            if (!_isPlayerInside) return;

            _currentWTMP = Instantiate(_WTMP, transform.position + _offsets, transform.rotation);
            var canvas = _currentWTMP.GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
            var tmp = _currentWTMP.GetComponentInChildren<TMP_Text>();
            var text = ElevatorDirection == ElevatorButtonDirection.Upward ? "Move Up" : "Move Down";
            tmp.text = text;
        }

        public void OnPointedAway() {
            if (!_isPlayerInside) return;

            Destroy(_currentWTMP);
        }
    }

    public enum ElevatorButtonDirection {
        Upward, Downward
    }
}
