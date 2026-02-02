namespace PingPingProduction.ProjectAnomaly.Interaction {
    public interface IInteractable {
        void Interact();

        void OnPointedAt();

        void OnPointedAway();
    }
}
