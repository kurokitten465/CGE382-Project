using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PingPingProduction.ProjectAnomaly {
    public class AudioFlicker : MonoBehaviour {
        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip[] clips;

        [Header("Timing (seconds)")]
        public float minInterval = 0.05f;
        public float maxInterval = 0.2f;

        [Header("Burst")]
        [Tooltip("Chance (0-1) to play 2-3 sounds rapidly in a row")]
        public float burstChance = 0.2f;

        private CancellationTokenSource _cts;

        void Start() {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
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
                        if (clips.Length > 0)
                            audioSource.PlayOneShot(clips[UnityEngine.Random.Range(0, clips.Length)]);

                        float wait = UnityEngine.Random.Range(minInterval, maxInterval);
                        await UniTask.WaitForSeconds(wait, cancellationToken: token);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}
