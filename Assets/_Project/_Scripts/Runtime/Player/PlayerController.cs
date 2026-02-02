using System;
using UnityEngine;
using PingPingProduction.ProjectAnomaly.Core.Input;

namespace PingPingProduction.ProjectAnomaly.Player {
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour {
        [Header("Movement Settings")]
        [SerializeField]
        private float _walkSpeed = 5f;
        [SerializeField]
        private float _sprintSpeed = 10f;

        [Header("Dependencies")]
        [SerializeField]
        private InputReader _inputReader;
        
        private CharacterController _characterCtl;
        private Transform _camTranform;
        private Vector3 _playerVelocity = Vector3.zero;
        private const float GRAVITY = -9.81f;

        public Action<Collider> OnPlayerMoved;

        private void Awake() {
            _characterCtl = GetComponent<CharacterController>();
            _camTranform = Camera.main.transform;
        }

        private void OnEnable() {
            _inputReader.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable() {
            _inputReader.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void FixedUpdate() {
            HandleMovement();
        }

        private void HandleMovement() {
            var input = _inputReader.GetMovement();

            var move = new Vector3(input.x, 0f, input.y);
            move = _camTranform.forward * move.z + _camTranform.right * move.x;
            var speed =  _inputReader.IsSprinting() ? _sprintSpeed : _walkSpeed;

            _characterCtl.Move(move * speed);

            var yVelocity = _characterCtl.isGrounded ? 0f : GRAVITY;
            _playerVelocity.y = yVelocity;

            _characterCtl.Move(_playerVelocity);
        }
    }
}
