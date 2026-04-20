using System;
using UnityEngine;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class AnomalyCardRegister : MonoBehaviour {
        public string ID;
        public HallwayConfig Config;
        public GameObject Card;
        public string Title;
        [TextArea] public string Desc; 
    }
}
