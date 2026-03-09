using System;
using UnityEngine;
using PingPingProduction.ProjectAnomaly.Core.Input;
using PingPingProduction.ProjectAnomaly.Utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class GameManager : MonoSingleton<GameManager> {
        [Header("Fading")]
        [field: SerializeField] public CanvasGroup FadingCanvas { get; private set; }

        [Header("Dependencies")]
        [SerializeField] InputReader _inputReader;

        [Header("Startup")]
        [SerializeField] bool _useStartup;
        [SerializeField] string _startupScene;

        // Exposing Member
        public bool IsPause { get; private set; } = false;

        // Init
        protected override void Awake() {
            base.Awake();
            Pause();
            if (_useStartup)
                OnLoading().Forget();
        }

        // GamePaused
        public static Action<bool> OnGamePaused;
        public bool Pause() {
            IsPause = !IsPause;

            if (IsPause) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
                _inputReader.SwitchMapTo(InputReader.ActionMap.UI);
            }
            else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;
                _inputReader.SwitchMapTo(InputReader.ActionMap.Player);
            }

            OnGamePaused?.Invoke(IsPause);

            return IsPause;
        }

        // GameEnded
        public static Action OnGameEnded;
        public void End() {
            OnGameEnded?.Invoke();
        }

        public async UniTaskVoid OnLoading() {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_startupScene);

            // Stop the scene from activating immediately
            asyncLoad.allowSceneActivation = false;

            // Wait until the load is 90% complete (Unity's internal loading stops at 0.9)
            while (asyncLoad.progress < 0.9f) {
                // Update UI, etc.
                await UniTask.Yield();
            }

            // Now, allow the scene to activate (Awake/Start methods will run now, which can still cause a small hitch)
            asyncLoad.allowSceneActivation = true;

            // The rest of the while loop handles the final activation
            while (!asyncLoad.isDone) {
                await UniTask.Yield();
            }

            await FadingCanvas
                    .DOFade(0f, 5f)
                    .From(1f, true)
                    .AsyncWaitForCompletion()
                    .AsUniTask();
        }
    }
}
