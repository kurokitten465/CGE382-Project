using UnityEngine;

namespace PingPingProduction.ProjectAnomaly.Interaction
{
    public class ElevatorInfoTrigger : MonoBehaviour, IInteractable
    {
        [Header("Dependencies")]
        [SerializeField] ElevatorTrigger _elevatorTrigger;
        [SerializeField] GameObject _WTMP;
        [SerializeField] Vector3 _offsets;

        GameObject _currentWTMP;

        public bool Enable = true;

        bool _isPlayerInside = false;

        void OnEnable() {
            _elevatorTrigger.OnLiftTriggered += OnPlayerFullyInside;
        }

        void OnDisable() {
            _elevatorTrigger.OnLiftTriggered -= OnPlayerFullyInside;
        }

        public void OnPlayerFullyInside(bool isInside) => _isPlayerInside = isInside;

        public void Interact() {
            
        }

        public void OnPointedAt() {
            if (!_isPlayerInside) return;

            _currentWTMP = Instantiate(_WTMP, transform.position + _offsets, transform.rotation);
            var canvas = _currentWTMP.GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
        }

        public void OnPointedAway() {
            if (!_isPlayerInside) return;

            Destroy(_currentWTMP);
        }
    }
}
