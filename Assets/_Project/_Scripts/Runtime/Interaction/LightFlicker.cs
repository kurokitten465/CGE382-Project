using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PingPingProduction.ProjectAnomaly {
    public class LightFlicker : MonoBehaviour {
        [Header("Intensity")]
        public float minIntensity = 0.8f;
        public float maxIntensity = 1.2f;

        [Header("Timing (seconds)")]
        public float minInterval = 0.05f;
        public float maxInterval = 0.2f;

        [Header("Burst")]
        [Tooltip("Chance (0-1) to flicker 2-3 times rapidly in a row")]
        public float burstChance = 0.2f;

        private Light _light;
        private CancellationTokenSource _cts;

        void Start() {
            _light = GetComponent<Light>();
            _cts = new CancellationTokenSource();
            FlickerLoop(_cts.Token).Forget();
        }

        void OnDestroy() => _cts?.Cancel();

        async UniTaskVoid FlickerLoop(CancellationToken token) {
            try {
                while (true) {
                    token.ThrowIfCancellationRequested();

                    int flickers = UnityEngine.Random.value < burstChance ? UnityEngine.Random.Range(2, 4) : 1;

                    for (int i = 0; i < flickers; i++) {
                        _light.intensity = UnityEngine.Random.Range(minIntensity, maxIntensity);
                        float wait = UnityEngine.Random.Range(minInterval, maxInterval);
                        await UniTask.WaitForSeconds(wait, cancellationToken: token);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}
