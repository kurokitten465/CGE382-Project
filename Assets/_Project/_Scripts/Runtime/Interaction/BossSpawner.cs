using System.Collections.Generic;
using System.Linq;
using PingPingProduction.ProjectAnomaly.Core;
using PingPingProduction.ProjectAnomaly.Interaction;
using UnityEngine;

namespace PingPingProduction.ProjectAnomaly
{
    public class BossSpawner : MonoBehaviour
    {
        bool _isTriggered = false;

        [SerializeField] GameObject _hinaBoss;
        [SerializeField] GameObject _yuukiBoss;

        List<ElevatorButtonTrigger> _buttonTriggers = new();

        void Start() {
            _buttonTriggers = FindObjectsByType<ElevatorButtonTrigger>(sortMode: FindObjectsSortMode.None).ToList();
            _buttonTriggers.ForEach(e => e.Enable = false);
        }

        void Update() {
            if (!_isTriggered) return;

            if (ProgressManager.IsResolving) {
                _hinaBoss.GetComponent<BossMover>().Cancel();
                _yuukiBoss.GetComponent<BossMover>().Cancel();
            }
        }

        void OnTriggerEnter(Collider other) {
            if (_isTriggered) return;

            if (other.CompareTag("Player")) {
                _buttonTriggers.ForEach(e => e.Enable = true);
                _isTriggered = true;

                if (GameManager.Instance.PlayerLastElevator == ElevatorType.Hina) {
                    _hinaBoss.SetActive(true);
                }
                else {
                    _yuukiBoss.SetActive(true);
                }
            }
        }
    }
}
