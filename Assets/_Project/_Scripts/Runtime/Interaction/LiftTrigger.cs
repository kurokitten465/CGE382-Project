using UnityEngine;
using PingPingProduction.ProjectAnomaly.Core;

namespace PingPingProduction.ProjectAnomaly.Interaction {
    [RequireComponent(typeof(Collider))]
    public class LiftTrigger : MonoBehaviour {
        [SerializeField]
        private LiftIdentity _liftID;

        [SerializeField]
        private RoomManager _rm;

        [SerializeField]
        private LayerMask _layerMask;

        private Collider _collider;

        private void Awake() {
            _collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider collider) {
            if (!collider.TryGetComponent<CharacterController>(out _)) return;
        }

        private void OnTriggerStay(Collider collider) {
            if (!collider.TryGetComponent<CharacterController>(out var characterController)) return;

            if (GameManager.Instance.CurremtGameState != GameState.Observe) return;

            if (IsPlayerFullyInside(characterController)) {
                GameManager.Instance.TransitionState(GameState.Resolve);

                if (GameManager.Instance.CurremtGameState != GameState.Resolve) return;

                _rm.OnLiftEntered(_liftID);
            }
        }

        private void OnTriggerExit(Collider collider) {
            if (!collider.TryGetComponent<CharacterController>(out _)) return;
            
            GameManager.Instance.TransitionState(GameState.Observe);
            GameManager.Instance.SetLastestRideOnLift(_liftID);
        }

        public bool IsPlayerFullyInside(CharacterController character) {
            Vector3 center = character.transform.position + character.center;
            float radius = character.radius;
            float height = character.height;

            Vector3 point1 = center + Vector3.up * (height / 2f - radius);
            Vector3 point2 = center + Vector3.down * (height / 2f - radius);

            Collider[] overlaps = Physics.OverlapCapsule(point1, point2, radius, _layerMask);

            foreach (var col in overlaps) {
                if (col != _collider)
                    return false;
            }

            return true;
        }
    }
}
