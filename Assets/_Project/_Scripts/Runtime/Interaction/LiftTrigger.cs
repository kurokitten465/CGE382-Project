using UnityEngine;
using PingPingProduction.ProjectAnomaly.Core;
using System;
using System.Collections;

namespace PingPingProduction.ProjectAnomaly.Interaction {
    [RequireComponent(typeof(Collider))]
    public class LiftTrigger : MonoBehaviour {
        [SerializeField]
        private LiftIdentity _liftID;

        [SerializeField]
        private RoomManager _rm;

        private Collider _liftCollider;

        private Coroutine _triggerCoroutine;

        private void Awake() {
            _liftCollider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider collider) {
            if (!collider.TryGetComponent(out CharacterController controller))
                return;

            if (GameManager.Instance.CurremtGameState == GameState.Observe)
                _triggerCoroutine = StartCoroutine(OnLiftTriggered(controller));
        }

        private IEnumerator OnLiftTriggered(CharacterController player) {
            GameManager.Instance.TransitionState(GameState.Resolve);
            
            if (!IsPlayerFullyInside(player)) {
                yield return null;
            }

            _rm.OnLiftEntered(_liftID);
        }

        private void OnTriggerExit(Collider collider) {
            if (_triggerCoroutine != null)
                StopCoroutine(_triggerCoroutine);

            GameManager.Instance.TransitionState(GameState.Observe);
            GameManager.Instance.SetLastestRideOnLift(_liftID);
        }

        private bool IsPlayerFullyInside(CharacterController character) {
            return _liftCollider.bounds.Contains(character.bounds.min) &&
                   _liftCollider.bounds.Contains(character.bounds.max);
        }
    }
}
