using PingPingProduction.ProjectAnomaly.Core.Input;
using PingPingProduction.ProjectAnomaly.Interaction;
using UnityEngine;

namespace PingPingProduction.ProjectAnomaly.Player {
    public class PlayerInteraction : MonoBehaviour {
        [Header("Dependencies")]
        [SerializeField] private InputReader _inputReader;

        [Header("Raycast Settings")]
        [SerializeField] private float _raycastDistance = 100f;
        [SerializeField] private LayerMask _interactableLayer;

        [Header("Debug")]
        [SerializeField] private Light _light;
        [SerializeField] private bool _showDebugRay = true;

        private Camera _camera;
        private IInteractable _currentPointedInteractable;

        private void Start() {
            _camera = Camera.main;
        }

        private void OnEnable() {
            if (_inputReader != null) {
                _inputReader.OnInteractedEvent += OnInteractInput;
            }
        }

        private void OnDisable() {
            if (_inputReader != null) {
                _inputReader.OnInteractedEvent -= OnInteractInput;
            }
        }

        private void Update() {
            PerformRaycast();
        }

        private void PerformRaycast() {
            Ray ray = _camera.ScreenPointToRay(_camera.pixelRect.center);

            if (_showDebugRay) {
                Debug.DrawRay(ray.origin, ray.direction * _raycastDistance, Color.green);
            }

            if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _interactableLayer)) {
                if (hit.collider.TryGetComponent<IInteractable>(out var interactable)) {
                    if (_currentPointedInteractable != interactable) {
                        _currentPointedInteractable?.OnPointedAway();
                        _currentPointedInteractable = interactable;
                        _currentPointedInteractable.OnPointedAt();
                    }
                } else {
                    ClearPointedInteractable();
                }
            } else {
                ClearPointedInteractable();
            }
        }

        private void OnInteractInput() {
            _currentPointedInteractable?.Interact();
        }

        private void ClearPointedInteractable() {
            _currentPointedInteractable?.OnPointedAway();
            _currentPointedInteractable = null;
        }
    }
}
