using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class SequenceMover : MonoBehaviour {
        [Header("X Positions")]
        public float[] positions = { -0.355f, -0.215f, -0.075f, 0.065f, 0.2f, 0.345f };

        [Header("Movement")]
        public float moveDuration = 0.4f;
        public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private int _currentIndex = 0;
        private bool _isMoving = false;
        private CancellationTokenSource _cts;

        void Awake() {
            // Snap to first position on start
            SetX(positions[0]);
        }

        void OnDestroy() => _cts?.Cancel();

        // ── Public API ────────────────────────────────────────────────────────

        public async UniTask MoveForward(CancellationToken externalToken = default) {
            if (_isMoving) return;
            if (_currentIndex >= positions.Length - 1) return;

            await MoveTo(_currentIndex + 1, externalToken);
        }

        public async UniTask MoveToStart(CancellationToken externalToken = default) {
            if (_isMoving) return;
            if (_currentIndex <= 0) return;

            await MoveTo(0, externalToken);
        }

        public void Reset() {
            Cancel();
            _currentIndex = 0;
            SetX(positions[0]);
        }

        // ── Internal ──────────────────────────────────────────────────────────

        async UniTask MoveTo(int targetIndex, CancellationToken externalToken) {
            Cancel();
            _cts = new CancellationTokenSource();

            var token = externalToken == default
                ? _cts.Token
                : CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, externalToken).Token;

            float fromX = transform.localPosition.x;
            float toX = positions[targetIndex];

            _isMoving = true;

            try {
                float elapsed = 0f;

                while (elapsed < moveDuration) {
                    token.ThrowIfCancellationRequested();

                    elapsed += Time.deltaTime;
                    float t = moveCurve.Evaluate(Mathf.Clamp01(elapsed / moveDuration));
                    SetX(Mathf.LerpUnclamped(fromX, toX, t));

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }

                SetX(toX);
                _currentIndex = targetIndex;
            }
            catch (OperationCanceledException) { }
            finally {
                _isMoving = false;
            }
        }

        void SetX(float x) {
            var pos = transform.localPosition;
            pos.x = x;
            transform.localPosition = pos;
        }

        void Cancel() {
            _cts?.Cancel();
            _cts = null;
            _isMoving = false;
        }
    }
}
