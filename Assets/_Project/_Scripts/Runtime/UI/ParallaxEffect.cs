using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PingPingProduction.ProjectAnomaly.UI {
    public class ParallaxEffect : MonoBehaviour {
        [System.Serializable]
        public class ParallaxLayer {
            public RectTransform rectTransform;
            [Range(-1f, 1f)]
            [Tooltip("Positive = moves with cursor. Negative = moves against. 0 = static.")]
            public float depth = 0.5f;
            [HideInInspector] public Vector2 originalAnchoredPosition;
        }

        [Header("Frame Reference")]
        [Tooltip("RectTransform of the frame — defines card bounds for input sampling")]
        public RectTransform frameRect;

        [Header("Layers")]
        public List<ParallaxLayer> layers = new();

        [Header("Layer Translation")]
        [Tooltip("Max pixel offset for a layer at depth = 1")]
        public float maxOffset = 40f;

        [Header("Root Rotation")]
        [Tooltip("Max tilt angle in degrees — keep this small (3–6) for a lenticular feel")]
        public float maxRotation = 4f;
        [Tooltip("Axis weight: X tilts up/down, Y tilts left/right. Usually keep both at 1.")]
        public Vector2 rotationAxisScale = Vector2.one;

        [Header("Smoothing")]
        public float smoothSpeed = 8f;
        public float recenterSpeed = 5f;

        [Header("Input")]
        public InputActionReference holdAction;
        public InputActionReference pointerPositionAction;

        private bool _isHolding;
        private Vector2 _targetOffset;
        private Vector2 _currentOffset;
        private Camera _uiCamera;
        private Canvas _canvas;

        // ─────────────────────────────────────────────

        void Awake() {
            _canvas = GetComponentInParent<Canvas>();
            _uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null : _canvas.worldCamera;

            foreach (var layer in layers)
                if (layer.rectTransform != null)
                    layer.originalAnchoredPosition = layer.rectTransform.anchoredPosition;

            if (holdAction != null) {
                holdAction.action.performed += OnHolding;
                holdAction.action.canceled += OnCanceled;
                //holdAction.action.Enable();
            }

            /* if (pointerPositionAction != null)
                pointerPositionAction.action.Enable(); */
        }

        void OnHolding(InputAction.CallbackContext context) {
            _isHolding = true;
        }

        void OnCanceled(InputAction.CallbackContext context) {
            _isHolding = false;
            _targetOffset = Vector2.zero;
        }

        void Update() {
            if (_isHolding) {
                Vector2 screenPos = pointerPositionAction != null
                    ? pointerPositionAction.action.ReadValue<Vector2>()
                    : (Vector2)Mouse.current.position.ReadValue();

                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        frameRect, screenPos, _uiCamera, out Vector2 local)) {
                    Vector2 size = frameRect.rect.size;
                    Vector2 normalized = new Vector2(
                        Mathf.Clamp(local.x / (size.x * 0.5f), -1f, 1f),
                        Mathf.Clamp(local.y / (size.y * 0.5f), -1f, 1f)
                    );
                    _targetOffset = normalized * maxOffset;
                }
            }

            float speed = _isHolding ? smoothSpeed : recenterSpeed;
            _currentOffset = Vector2.Lerp(_currentOffset, _targetOffset, Time.deltaTime * speed);

            // ── Subtle root rotation ──────────────────────────────────────────
            // Tilt X axis (up/down) driven by vertical cursor offset
            // Tilt Y axis (left/right) driven by horizontal cursor offset
            float normalizedX = Mathf.Approximately(maxOffset, 0f) ? 0f : _currentOffset.x / maxOffset;
            float normalizedY = Mathf.Approximately(maxOffset, 0f) ? 0f : _currentOffset.y / maxOffset;

            float rotX = -normalizedY * maxRotation * rotationAxisScale.x; // pitch
            float rotY = normalizedX * maxRotation * rotationAxisScale.y; // yaw
            transform.localRotation = Quaternion.Euler(rotX, rotY, 0f);

            // ── Layer translation (parallax peek) ────────────────────────────
            foreach (var layer in layers) {
                if (layer.rectTransform == null) continue;
                layer.rectTransform.anchoredPosition =
                    layer.originalAnchoredPosition + _currentOffset * layer.depth;
            }
        }

        void OnDestroy() {
            holdAction.action.performed -= OnHolding;
            holdAction.action.canceled -= OnCanceled;
        }
        /* [System.Serializable]
        public class ParallaxLayer {
            public RectTransform rectTransform;
            [Range(-1f, 1f)]
            [Tooltip("Positive = moves with cursor. Negative = moves against. 0 = static.")]
            public float depth = 0.5f;
            [HideInInspector] public Vector2 originalAnchoredPosition;
        }

        [Header("Frame Reference")]
        [Tooltip("RectTransform of the frame/mask area — defines the card bounds")]
        public RectTransform frameRect;

        [Header("Layers")]
        public List<ParallaxLayer> layers = new();

        [Header("Parallax Settings")]
        [Tooltip("Max pixel offset for a layer at depth = 1")]
        public float maxOffset = 40f;
        [Tooltip("Smoothing while held")]
        public float smoothSpeed = 8f;
        [Tooltip("Smoothing when released — snaps back to center")]
        public float recenterSpeed = 5f;

        [Header("Input")]
        public InputActionReference holdAction;
        public InputActionReference pointerPositionAction;

        private bool _isHolding;
        private Vector2 _targetOffset;
        private Vector2 _currentOffset;
        private Camera _uiCamera;
        private Canvas _canvas;

        void Awake() {
            _canvas = GetComponentInParent<Canvas>();
            _uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null : _canvas.worldCamera;

            foreach (var layer in layers)
                if (layer.rectTransform != null)
                    layer.originalAnchoredPosition = layer.rectTransform.anchoredPosition;

            if (holdAction != null) {
                holdAction.action.performed += _ => _isHolding = true;
                holdAction.action.canceled += _ => { _isHolding = false; _targetOffset = Vector2.zero; };
                holdAction.action.Enable();
            }

            if (pointerPositionAction != null)
                pointerPositionAction.action.Enable();
        }

        void Update() {
            if (_isHolding) {
                Vector2 screenPos = pointerPositionAction != null
                    ? pointerPositionAction.action.ReadValue<Vector2>()
                    : (Vector2)UnityEngine.InputSystem.Mouse.current.position.ReadValue();

                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        frameRect, screenPos, _uiCamera, out Vector2 local)) {
                    Vector2 size = frameRect.rect.size;
                    // Normalize pointer to -1..1 within the frame bounds
                    Vector2 normalized = new Vector2(
                        Mathf.Clamp(local.x / (size.x * 0.5f), -1f, 1f),
                        Mathf.Clamp(local.y / (size.y * 0.5f), -1f, 1f)
                    );
                    _targetOffset = normalized * maxOffset;
                }
            }

            float speed = _isHolding ? smoothSpeed : recenterSpeed;
            _currentOffset = Vector2.Lerp(_currentOffset, _targetOffset, Time.deltaTime * speed);

            // Translate each layer — deeper layers move more, creating the parallax peek effect
            foreach (var layer in layers) {
                if (layer.rectTransform == null) continue;
                layer.rectTransform.anchoredPosition =
                    layer.originalAnchoredPosition + _currentOffset * layer.depth;
            }

            // No rotation on the card root — the frame stays perfectly still
            // The illusion of depth comes purely from differential 2D translation
        }

        void OnDestroy() {
            holdAction?.action.Disable();
            pointerPositionAction?.action.Disable();
        } */
    }
}