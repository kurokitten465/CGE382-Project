using System;
using UnityEngine;

namespace PingPingProduction.ProjectAnomaly.Core {
    [CreateAssetMenu(fileName = "HallwayConfig", menuName = "Project/HallwayConfig")]
    public class HallwayConfig : ScriptableObject {
        public GameObject SceneHolder;
        public bool IsAnomaly;
    }

    [Serializable]
    public readonly struct RuntimeHallwayConfig {
        public readonly GameObject HallwayPrefab;        
        public readonly bool IsAnomaly;

        public RuntimeHallwayConfig(GameObject hallwayPrefab, bool isAnomaly) {
            HallwayPrefab = hallwayPrefab;
            IsAnomaly = isAnomaly;
        }
    }
}
