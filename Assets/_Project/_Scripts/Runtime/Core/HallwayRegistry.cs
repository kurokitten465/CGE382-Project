using System;
using System.Collections.Generic;
using UnityEngine;

namespace PingPingProduction.ProjectAnomaly.Core {
    [CreateAssetMenu(fileName = "hallway_registry", menuName = "Project/HallwayRegistry")]
    public class HallwayRegistry : ScriptableObject {
        public List<Hallway> Hallways;
    }

    [Serializable]
    public struct Hallway {
        public HallwayConfig HallwayConfig;
        [Range(1, 100)] public byte Quantity;
    }
}
