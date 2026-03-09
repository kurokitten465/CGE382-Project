using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PingPingProduction.ProjectAnomaly.Core.Input {
    [CreateAssetMenu(fileName = "InputReader", menuName = "Project/InputReader")]
    public class InputReader : ScriptableObject, GameInputActions.IPlayerActions, GameInputActions.IUIActions {
        public enum ActionMap {
            Player, UI
        }

        private GameInputActions _inputActions;

        #region Events
        public Action<InputAction.CallbackContext> OnPlayerMove;
        public Action<InputAction.CallbackContext> OnPlayerLook;
        public Action<InputAction.CallbackContext> OnPlayerInteract;
        public Action<InputAction.CallbackContext> OnPlayerSprint;
        public Action<InputAction.CallbackContext> OnPlayerPaused;
        #endregion

        #region Initialize
        void OnEnable() {
            _inputActions = new();

            _inputActions.Player.AddCallbacks(this);
            _inputActions.UI.AddCallbacks(this);
        }

        void OnDestroy() {
            _inputActions.Player.RemoveCallbacks(this);
            _inputActions.UI.RemoveCallbacks(this);

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

        public void OnPause(InputAction.CallbackContext context) {
            OnPlayerPaused?.Invoke(context);

            if (context.phase != InputActionPhase.Canceled) return;

            GameManager.Instance.Pause();
        }
        #endregion

        #region  Active Action Map
        public void Active(ActionMap map) {
            if (map == ActionMap.Player) {
                _inputActions.Player.Enable();
            }
            else {
                _inputActions.UI.Enable();
            }
        }

        public void Deactive(ActionMap map) {
            if (map == ActionMap.Player) {
                _inputActions.Player.Disable();
            }
            else {
                _inputActions.UI.Disable();
            }
        }

        public void SwitchMapTo(ActionMap map) {
            if (map == ActionMap.Player) {
                _inputActions.Player.Enable();
                _inputActions.UI.Disable();
            }
            else {
                _inputActions.Player.Disable();
                _inputActions.UI.Enable();
            }
        }

        public void DeactiveAll() {
            _inputActions.Player.Disable();
            _inputActions.UI.Disable();
        }

        public void OnNavigate(InputAction.CallbackContext context) {
            
        }

        public void OnSubmit(InputAction.CallbackContext context) {
            
        }

        public void OnCancel(InputAction.CallbackContext context) {
            
        }

        public void OnPoint(InputAction.CallbackContext context) {
            
        }

        public void OnClick(InputAction.CallbackContext context) {
            
        }

        public void OnRightClick(InputAction.CallbackContext context) {
            
        }

        public void OnMiddleClick(InputAction.CallbackContext context) {
            
        }

        public void OnScrollWheel(InputAction.CallbackContext context) {
            
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context) {
            
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) {
            
        }

        public void OnUnPause(InputAction.CallbackContext context) {
            if (context.phase != InputActionPhase.Canceled) return;

            GameManager.Instance.Pause();
        }
        #endregion
    }
}
