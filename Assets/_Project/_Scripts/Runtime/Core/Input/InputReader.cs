using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PingPingProduction.ProjectAnomaly.Core.Input {
    [CreateAssetMenu(fileName = "InputReader", menuName = "Project/InputReader")]
    public class InputReader : ScriptableObject {
        private GameInputActions _inputActions;

        public Action OnInteractedEvent;

        private void OnEnable() {
            _inputActions ??= new();
        }

        public void SetActive(bool active) {
            _inputActions ??= new();

            if (active) {
                _inputActions.Enable();

                _inputActions.Player.Interact.started += OnInteracted;
            }
            else {
                _inputActions.Enable();
                
                _inputActions.Player.Interact.started -= OnInteracted;
            }
        }

        private void OnInteracted(InputAction.CallbackContext context) {
            if (context.phase != InputActionPhase.Started) return;
            
            OnInteractedEvent?.Invoke();
        }

        public Vector2 GetMovement() =>
            _inputActions.Player.Move.ReadValue<Vector2>();

        public Vector2 GetMouseDelta() =>
            _inputActions.Player.Look.ReadValue<Vector2>();

        public bool IsSprinting() =>
            _inputActions.Player.Sprint.IsInProgress();
    }
}
