using System;
using System.Collections.Generic;
using UnityEngine;

namespace PingPingProduction.ProjectAnomaly.Core {
    [CreateAssetMenu(fileName = "HallwayDeck", menuName = "Project/HallwayDeck")]
    public class HallwayDeck : ScriptableObject {
        [SerializeField]
        private List<Hallway> _hallways;

        public IReadOnlyList<Hallway> Hallways => _hallways;

        private void OnValidate() {
            foreach (var hallway in _hallways) {
                hallway.ID = hallway.Config.SceneHolder.name;
            }
        }
    }

    [Serializable]
    public class Hallway {
        public string ID;
        public HallwayConfig Config;
        [Range(1, 100)]
        public int Quantity = 1;
    }
}
