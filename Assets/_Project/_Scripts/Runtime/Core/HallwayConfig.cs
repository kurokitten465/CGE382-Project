using UnityEngine;

namespace PingPingProduction.ProjectAnomaly.Core {
    [CreateAssetMenu(fileName = "hallway_new", menuName = "Project/HallwayConfig")]
    public class HallwayConfig : ScriptableObject {
        public GameObject HallwayPrefab;
        public bool IsAnomaly;
    }
}
