using UnityEngine;

namespace PingPingProduction.ProjectAnomaly.Core {
    [CreateAssetMenu(fileName = "HallwayConfig", menuName = "Project/HallwayConfig")]
    public class HallwayConfig : ScriptableObject {
        public string SceneHolder;
        public bool IsAnomaly;

        #if UNITY_EDITOR
        public UnityEditor.SceneAsset SceneAsset;

        private void OnValidate() {
            if (SceneAsset != null) {
                SceneHolder = SceneAsset.name;
            }
            else {
                SceneHolder = string.Empty;
            }
        }
#endif
    }

    public class RuntimeHallwayConfig {
        public readonly string SceneName;        
        public readonly bool IsAnomaly;
        public bool IsExplored { get; private set; }

        public RuntimeHallwayConfig(string scene, bool isAnomaly)
        {
            SceneName = scene;
            IsAnomaly = isAnomaly;
        }

        public void Explore() =>
            IsExplored = true;
    }
}
