using UnityEngine;
using PingPingProduction.ProjectAnomaly.Interaction;
using PingPingProduction.ProjectAnomaly.Core.Input;
using UnityEngine.InputSystem;
using System;

namespace PingPingProduction.ProjectAnomaly.Player {
    public class PlayerInteraction : MonoBehaviour {
        [Header("Dependencies")]
        [SerializeField] private InputReader _inputReader;

        [Header("Raycast Settings")]
        [SerializeField] private float _raycastDistance = 100f;
        [SerializeField] private LayerMask _interactableLayer;

        private Camera _camera;
        private IInteractable _currentPointedInteractable;

        private void Start() {
            _camera = Camera.main;
        }

        private void OnEnable() {
            _inputReader.OnPlayerInteract += OnInteractInput;
        }

        private void OnDisable() {
            _inputReader.OnPlayerInteract -= OnInteractInput;
        }

        private void Update() {
            PerformRaycast();
        }

        private void PerformRaycast() {
            Ray ray = _camera.ScreenPointToRay(_camera.pixelRect.center);

            Debug.DrawRay(ray.origin, ray.direction * _raycastDistance, Color.green);

            if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _interactableLayer)) {
                if (hit.collider.gameObject.TryGetComponent<IInteractable>(out var interactable)) {
                    if (_currentPointedInteractable != interactable || _currentPointedInteractable == null) {
                        _currentPointedInteractable?.OnPointedAway();
                        _currentPointedInteractable = interactable;
                        _currentPointedInteractable.OnPointedAt();
                    }
                }
                else {
                    ClearPointedInteractable();
                }
            }
            else {
                ClearPointedInteractable();
            }
        }

        private void OnInteractInput(InputAction.CallbackContext context) {
            if (context.phase != InputActionPhase.Canceled) return;

            _currentPointedInteractable?.Interact();
        }

        private void ClearPointedInteractable() {
            _currentPointedInteractable?.OnPointedAway();
            _currentPointedInteractable = null;
        }
    }
}
