using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PingPingProduction.ProjectAnomaly {
    public class BossMover : MonoBehaviour {
        [Header("Movement")]
        public Transform target;
        public float moveDuration = 0.5f;
        public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        void Start() {
            Launch().Forget();
        }

        private CancellationTokenSource _cts;

        void OnDestroy() => _cts?.Cancel();

        public void Cancel() => _cts?.Cancel();

        public async UniTask Launch(CancellationToken externalToken = default) {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            var token = externalToken == default
                ? _cts.Token
                : CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, externalToken).Token;

            Vector3 startPos = transform.position;
            float elapsed = 0f;

            try {
                while (elapsed < moveDuration) {
                    token.ThrowIfCancellationRequested();

                    elapsed += Time.deltaTime;
                    float t = moveCurve.Evaluate(Mathf.Clamp01(elapsed / moveDuration));

                    // Only interpolate X and Z — physics handles Y
                    float x = Mathf.LerpUnclamped(startPos.x, target.position.x, t);
                    float z = Mathf.LerpUnclamped(startPos.z, target.position.z, t);
                    transform.position = new Vector3(x, transform.position.y, z);

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}
