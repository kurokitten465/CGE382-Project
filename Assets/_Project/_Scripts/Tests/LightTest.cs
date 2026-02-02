using PingPingProduction.ProjectAnomaly.Interaction;
using UnityEngine;

public class LightTest : MonoBehaviour, IInteractable {
    [SerializeField] private Light _light;
    private bool _isOn = false;

    public void Interact() {
        _isOn = !_isOn;

        if (_isOn)
            _light.color = Color.green;
        else
            _light.color = Color.white;
    }

    public void OnPointedAt() {
        Debug.Log("Found Light");
    }

    public void OnPointedAway() {
        Debug.Log("Away from Light");
    }
}
