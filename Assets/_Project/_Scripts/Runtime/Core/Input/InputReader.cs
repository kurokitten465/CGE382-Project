using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PingPingProduction.ProjectAnomaly.Core.Input {
    [CreateAssetMenu(fileName = "InputReader", menuName = "Project/InputReader")]
    public class InputReader : ScriptableObject, GameInputActions.IPlayerActions {
        public enum ActionMap {
            Player, UI
        }

        private GameInputActions _inputActions;

        #region Events
        public Action<InputAction.CallbackContext> OnPlayerMove;
        public Action<InputAction.CallbackContext> OnPlayerLook;
        public Action<InputAction.CallbackContext> OnPlayerInteract;
        public Action<InputAction.CallbackContext> OnPlayerSprint;
        #endregion

        #region Initialize
        void OnEnable() {
            _inputActions = new();

            _inputActions.Player.AddCallbacks(this);
        }

        void OnDestroy() {
            _inputActions.Player.RemoveCallbacks(this);

            _inputActions.Dispose();
        }
        #endregion

        #region Input Handler
        public void OnMove(InputAction.CallbackContext context) {
            OnPlayerMove?.Invoke(context);
        }

        public void OnLook(InputAction.CallbackContext context) {
            OnPlayerLook?.Invoke(context);
        }

        public void OnInteract(InputAction.CallbackContext context) {
            OnPlayerInteract?.Invoke(context);
        }

        public void OnSprint(InputAction.CallbackContext context) {
            OnPlayerSprint?.Invoke(context);
        }
        #endregion

        #region  Active Action Map
        public void Active(ActionMap map) {
            if (map == ActionMap.Player) {
                _inputActions.Player.Enable();
            }
            else {
                _inputActions.InGameUI.Enable();
            }
        }

        public void Deactive(ActionMap map) {
            if (map == ActionMap.Player) {
                _inputActions.Player.Disable();
            }
            else {
                _inputActions.InGameUI.Disable();
            }
        }

        public void SwitchMapTo(ActionMap map) {
            if (map == ActionMap.Player) {
                _inputActions.Player.Enable();
                _inputActions.InGameUI.Disable();
            }
            else {
                _inputActions.Player.Disable();
                _inputActions.InGameUI.Enable();
            }
        }

        public void DeactiveAll() {
            _inputActions.Player.Disable();
            _inputActions.InGameUI.Disable();
        }
        #endregion
    }
}
