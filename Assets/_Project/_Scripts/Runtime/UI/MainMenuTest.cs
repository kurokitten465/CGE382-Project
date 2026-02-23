using UnityEngine;
using UnityEngine.SceneManagement;

namespace PingPingProduction.ProjectAnomaly.UI {
    public class MainMenuTest : MonoBehaviour {
        public string SceneName;

        public void OnClicked() {
            SceneManager.LoadScene(SceneName);
        }
    }
}
