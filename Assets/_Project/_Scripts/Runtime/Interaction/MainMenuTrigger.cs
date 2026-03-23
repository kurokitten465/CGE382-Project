using UnityEngine;
using UnityEngine.Events;

namespace PingPingProduction.ProjectAnomaly.Interaction {
    public class MainMenuTrigger : MonoBehaviour, IInteractable {
        public UnityEvent<MainMenuTrigger> OnInteracted;

        bool _isInteracted = false;

        public void Interact() {
            if (_isInteracted) return;

            _isInteracted = true;
            OnInteracted?.Invoke(this);
        }

        public void OnPointedAt() {

        }

        public void OnPointedAway() {
            _isInteracted = false;
        }
    }
}
