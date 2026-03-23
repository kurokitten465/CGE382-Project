using PingPingProduction.ProjectAnomaly.Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PingPingProduction.ProjectAnomaly.Interaction {
    public class MainMenuInteractor : MonoBehaviour {
        [SerializeField] InputReader _reader;

        void Start() {
            _reader.OnUIClicked += OnClicked;
        }

        void OnDestroy() {
            _reader.OnUIClicked -= OnClicked;
        }

        void OnClicked(InputAction.CallbackContext context) {
            if (context.phase != InputActionPhase.Started) return;

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit)) {
                if (hit.collider.TryGetComponent<IInteractable>(out var interactable)) {
                    interactable.Interact();
                }
            }
        }
    }
}
