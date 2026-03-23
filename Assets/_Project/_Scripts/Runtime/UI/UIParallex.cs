using System;
using System.Collections.Generic;
using PingPingProduction.ProjectAnomaly.Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PingPingProduction.ProjectAnomaly.UI {
    public class UIParallex : MonoBehaviour {
        [Header("References")]
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private RectTransform _contentRoot;

        [Header("Layers")]
        [SerializeField] private List<ParallaxLayer> _layers = new();

        [Header("Settings")]
        [SerializeField] private float _maxOffset = 30f;
        [SerializeField] private float _maxRotation = 10f;
        [SerializeField] private float _smoothTime = 0.1f;

        private RectTransform _rectTransform;

        private Vector2 _targetInput;
        private Vector2 _currentInput;
        private Vector2 _velocity;

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update() {
            UpdateInput();
            UpdateParallax();
        }

        // -------------------------
        // INPUT
        // -------------------------
        private void UpdateInput() {
            Vector2 normalized = GetNormalizedInput(Mouse.current.position.ReadValue());

            _targetInput = normalized;
            _currentInput = Vector2.SmoothDamp(
                _currentInput,
                _targetInput,
                ref _velocity,
                _smoothTime
            );
        }

        private Vector2 GetNormalizedInput(Vector2 screenPos) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform,
                screenPos,
                null,
                out var localPoint
            );

            var size = _rectTransform.rect.size;

            return new Vector2(
                Mathf.Clamp(localPoint.x / size.x, -1f, 1f),
                Mathf.Clamp(localPoint.y / size.y, -1f, 1f)
            );
        }

        // -------------------------
        // PARALLAX CORE
        // -------------------------
        private void UpdateParallax() {
            // 1. Rotate whole card (perspective illusion)
            _contentRoot.localRotation = Quaternion.Euler(
                _currentInput.y * _maxRotation,
                -_currentInput.x * _maxRotation,
                0f
            );

            // 2. Move layers
            foreach (var layer in _layers) {
                if (layer.Rect == null) continue;

                float depthFactor = 1f / Mathf.Max(layer.Depth, 0.01f);

                float offsetX = _currentInput.x * _maxOffset * depthFactor;
                float offsetY = _currentInput.y * _maxOffset * depthFactor;

                // Add curve (premium feel)
                offsetX *= (1f + Mathf.Abs(_currentInput.x));
                offsetY *= (1f + Mathf.Abs(_currentInput.y));

                layer.Rect.localPosition = new Vector3(offsetX, offsetY, 0f);

                // Optional: slight scale boost for foreground
                float scale = 1f + depthFactor * 0.1f;
                layer.Rect.localScale = Vector3.one * scale;
            }
        }

        // -------------------------
        // RUNTIME LAYER CREATION
        // -------------------------
        public void AddLayer(Sprite sprite, float depth) {
            GameObject obj = new GameObject("ParallaxLayer", typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(_contentRoot, false);

            var rect = obj.GetComponent<RectTransform>();

            // Stretch + oversize (IMPORTANT)
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            rect.sizeDelta = _viewport.rect.size * 1.5f;

            var image = obj.GetComponent<Image>();
            image.sprite = sprite;
            image.raycastTarget = false;

            _layers.Add(new ParallaxLayer {
                Rect = rect,
                Depth = depth
            });
        }
    }

    [Serializable]
    public class ParallaxLayer {
        public RectTransform Rect;
        public float Depth = 1f;              // 1 = near, higher = far
    }
}
