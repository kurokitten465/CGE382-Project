using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using UnityEngine;

namespace PingPingProduction.ProjectAnomaly.Core {
    [CreateAssetMenu(fileName = "hallway_registry", menuName = "Project/HallwayRegistry")]
    public class HallwayRegistry : ScriptableObject {
        [SerializeField] List<HallwayConfig> _hallways;
        public ReadOnlyCollection<HallwayConfig> Hallways => _hallways.AsReadOnly();
    }
}
