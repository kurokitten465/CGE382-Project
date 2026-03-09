using System;
using PingPingProduction.ProjectAnomaly.Core;
using Unity.Cinemachine;
using UnityEngine;

namespace PingPingProduction.ProjectAnomaly {
    public class CinemachineController : MonoBehaviour {
        [SerializeField] CinemachineInputAxisController _cinemachineInputAxis;

        void Start() {
            GameManager.OnGamePaused += OnPaused;
        }

        void OnDestroy() {
            GameManager.OnGamePaused -= OnPaused;
        }

        private void OnPaused(bool isPaused) {
            _cinemachineInputAxis.enabled = !isPaused;
        }
    }
}
