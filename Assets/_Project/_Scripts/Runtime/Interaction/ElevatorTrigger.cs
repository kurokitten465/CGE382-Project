using UnityEngine;
using PingPingProduction.ProjectAnomaly.Player;
using System;

namespace PingPingProduction.ProjectAnomaly.Interaction {
    public class ElevatorTrigger : MonoBehaviour {
        [Header("Properties")]
        [field: SerializeField] public ElevatorType Elevator { get; private set; }
        [SerializeField] Collider _triggerCollider;

        public Action<bool> OnLiftTriggered;

        bool _isTriggered = false;
        bool _isFullyInside = false;

        void OnTriggerStay(Collider collider) {
            if (_isTriggered) return;

            if (!collider.attachedRigidbody.TryGetComponent<PlayerController>(out var _)) return;

            _isFullyInside = IsPlayerFullyInside(collider);

            if (!_isFullyInside) return;

            _isTriggered = true;
            OnLiftTriggered?.Invoke(_isFullyInside);
        }

        void OnTriggerExit(Collider collider) {
            _isTriggered = false;
            _isFullyInside = false;
            OnLiftTriggered?.Invoke(_isFullyInside);
        }

        bool IsPlayerFullyInside(Collider player) {
            CapsuleCollider capsule = player as CapsuleCollider;
            if (capsule == null) {
                return _triggerCollider.bounds.Contains(player.bounds.min) &&
                       _triggerCollider.bounds.Contains(player.bounds.max);
            }

            float halfHeight = capsule.height / 2f;
            Vector3 topSpherePos = capsule.transform.TransformPoint(capsule.center + Vector3.up * halfHeight);
            Vector3 bottomSpherePos = capsule.transform.TransformPoint(capsule.center - Vector3.up * halfHeight);

            return _triggerCollider.bounds.Contains(topSpherePos) &&
                   _triggerCollider.bounds.Contains(bottomSpherePos) &&
                   _triggerCollider.bounds.Contains(capsule.transform.TransformPoint(capsule.center));
        }
    }

    public enum ElevatorType {
        Yuuki, Hina
    }
}
