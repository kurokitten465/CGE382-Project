using UnityEngine;
using PingPingProduction.ProjectAnomaly.Core;
using PingPingProduction.ProjectAnomaly.Player;
using Unity.Cinemachine;

namespace PingPingProduction.ProjectAnomaly.Interaction {
    public class ElevatorTrigger : MonoBehaviour {
        [field: SerializeField] public ElevatorType Elevator { get; private set; }
        [field: SerializeField] public CinemachineCamera ElevatorCam { get; private set; }
        [SerializeField] Collider _triggerCollider;
        
        
        [Header("Dependencies")]
        [SerializeField] RoomManager _roomManager;

        bool _isTriggered;

        void OnTriggerStay(Collider collider) {
            if (_isTriggered || GameManager.Instance.CurrentGameState != GameState.Exploring) return;

            if (!collider.TryGetComponent<PlayerController>(out var player))

            if (!IsPlayerFullyInside(collider)) return;
            _isTriggered = true;
            _roomManager.OnLiftTriggered(this, player);
        }

        void OnTriggerExit(Collider collider) {
            _isTriggered = false;
            GameManager.Instance.SetGameState(GameState.Exploring);
        }

        private bool IsPlayerFullyInside(Collider player) {
            return _triggerCollider.bounds.Contains(player.bounds.min) &&
                   _triggerCollider.bounds.Contains(player.bounds.max);
        }
    }

    public enum ElevatorType {
        Yuuki, Hina
    }
}
