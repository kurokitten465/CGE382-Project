using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PingPingProduction.ProjectAnomaly.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PingPingProduction.ProjectAnomaly.UI {
    public class GameMenuController : MonoBehaviour {
        [SerializeField] string _mainMenuScene;
        [SerializeField] GameObject _gameMenuPanel;

        void Start() {
            GameManager.OnGamePaused += OnPaused;
        }

        void OnDestroy() {
            GameManager.OnGamePaused -= OnPaused;
        }

        void OnPaused(bool isPaused) {
            _gameMenuPanel.SetActive(isPaused);
        }

        public void OnMainMenuButtonClicked() {
            OnLoading().Forget();
        }

        public void OnContinueButtonClicked() {
            GameManager.Instance.Pause();
        }

        public async UniTaskVoid OnLoading() {
            await GameManager.Instance.FadingCanvas
                    .DOFade(1f, 3f)
                    .From(0f, true)
                    .AsyncWaitForCompletion()
                    .AsUniTask();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_mainMenuScene);

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
        }

        public void OnExitButtonClicked() {
            GameManager.Instance.End();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
