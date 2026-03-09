using UnityEngine;
using PingPingProduction.ProjectAnomaly.Interaction;
using PingPingProduction.ProjectAnomaly.Core;

namespace PingPingProduction.ProjectAnomaly.Interaction {
    public class ElevatorButtonTrigger : MonoBehaviour, IInteractable {
        [Header("Settings")]
        [field: SerializeField] public ElevatorButtonDirection ElevatorDirection { get; private set; }

        [Header("Dependencies")]
        [SerializeField] ElevatorTrigger _elevatorTrigger;
        [SerializeField] Animator _elevatorBtnAnimator;

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
        }

        public void OnPointedAway() {
            if (!_isPlayerInside) return;
        }
    }

    public enum ElevatorButtonDirection {
        Upward, Downward
    }
}
