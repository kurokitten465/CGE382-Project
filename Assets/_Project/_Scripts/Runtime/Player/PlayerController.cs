using System;
using UnityEngine;
using UnityEngine.InputSystem;
using PingPingProduction.ProjectAnomaly.Core.Input;
using PingPingProduction.ProjectAnomaly.Core;

namespace PingPingProduction.ProjectAnomaly.Player {
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour {
        // References
        [Header("Movement")]
        [SerializeField] float _walkSpeed = 10f;
        [SerializeField] float _sprintSpeed = 15f;
        [SerializeField] float _groundDrag = 2f;

        [Header("Dependencies")]
        [SerializeField] InputReader _inputReader;
        [SerializeField] Transform _camTransform;

        // Caching
        Rigidbody _rb;
        GameManager _gameManager;

        // Mutable Variables
        Vector2 _input = Vector2.zero;
        float _moveSpeed;
        bool _isSprinting;

        void Awake() {
            _rb = GetComponent<Rigidbody>();
            _rb.linearDamping = _groundDrag;

            _gameManager = GameManager.Instance;
        }

        void OnEnable() {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _inputReader.OnPlayerMove += GetMovement;
            _inputReader.OnPlayerSprint += GetSprint;

            _gameManager.OnGamePaused += OnPaused;

            _inputReader.Active(InputReader.ActionMap.Player);
        }

        void OnDisable() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _inputReader.OnPlayerMove -= GetMovement;
            _inputReader.OnPlayerSprint -= GetSprint;

            _gameManager.OnGamePaused -= OnPaused;

            _inputReader.Deactive(InputReader.ActionMap.Player);
        }

        void FixedUpdate() {
            Move();
            SpeedControl();
        }

        void Move() {
            var camForward = _camTransform.forward;
            var camRight = _camTransform.right;
            
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            
            var moveDirection = camForward * _input.y + camRight * _input.x;
            _moveSpeed = _isSprinting ? _sprintSpeed : _walkSpeed;
            _rb.AddForce(_moveSpeed * 10f * moveDirection.normalized, ForceMode.Force);
        }

        void SpeedControl() {
            var flatVal = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

            if (flatVal.magnitude > _moveSpeed) {
                var limitedSpeed = flatVal.normalized * _moveSpeed;
                _rb.linearVelocity = new Vector3(limitedSpeed.x, _rb.linearVelocity.y, limitedSpeed.z);
            }
        }

        void GetMovement(InputAction.CallbackContext context) {
            _input = context.ReadValue<Vector2>();
        }

        private void GetSprint(InputAction.CallbackContext context) {
            _isSprinting = context.phase == InputActionPhase.Performed;
        }

        private void OnPaused(IPauseContext context, bool isPaused) {
            if (isPaused) {
                _inputReader.DeactiveAll();
            }
            else {
                _inputReader.Active(InputReader.ActionMap.Player);
            }
        }
    }
}
