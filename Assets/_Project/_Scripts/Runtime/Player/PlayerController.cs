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

        [Header("Footsteps")]
        [SerializeField] AudioSource _audio;
        [SerializeField] float _walkStepInterval = 0.5f;
        [SerializeField] float _sprintStepInterval = 0.35f;
        [SerializeField] AudioClip[] _carpetSteps;
        [SerializeField] AudioClip[] _woodSteps;

        [Header("Dependencies")]
        [SerializeField] InputReader _inputReader;

        // Caching
        Transform _camTransform;
        Rigidbody _rb;

        // Mutable Variables
        Vector2 _input = Vector2.zero;
        float _moveSpeed;
        bool _isSprinting;
        float _stepTimer;

        void Awake() {
            _rb = GetComponent<Rigidbody>();
            _rb.linearDamping = _groundDrag;
        }

        void Start() {
            _camTransform = Camera.main.transform;
        }

        void OnEnable() {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _inputReader.OnPlayerMove += GetMovement;
            _inputReader.OnPlayerSprint += GetSprint;
        }

        void OnDisable() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _inputReader.OnPlayerMove -= GetMovement;
            _inputReader.OnPlayerSprint -= GetSprint;
        }

        void FixedUpdate() {
            Move();
            SpeedControl();
            HandleFootsteps();
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

        void HandleFootsteps() {
            Vector3 flatVel = new(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);

            if (flatVel.magnitude < 0.15f) return;

            float interval = _isSprinting ? _sprintStepInterval : _walkStepInterval;

            _stepTimer -= Time.fixedDeltaTime;

            if (_stepTimer > 0) return;

            _stepTimer = interval;

            PlayFootstep();
        }

        void PlayFootstep() {
            if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
                return;

            AudioClip[] clips = null;

            if (hit.collider.CompareTag("Carpet"))
                clips = _carpetSteps;
            else if (hit.collider.CompareTag("Wood"))
                clips = _woodSteps;

            if (clips == null || clips.Length == 0) return;

            _audio.PlayOneShot(clips[UnityEngine.Random.Range(0, clips.Length)]);
        }

        void GetMovement(InputAction.CallbackContext context) {
            _input = context.ReadValue<Vector2>();
        }

        private void GetSprint(InputAction.CallbackContext context) {
            _isSprinting = context.phase == InputActionPhase.Performed;
        }
    }
}
