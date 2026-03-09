using Cysharp.Threading.Tasks;
using DG.Tweening;
using PingPingProduction.ProjectAnomaly.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PingPingProduction.ProjectAnomaly.UI {
    public class MainMenuController : MonoBehaviour {
        [SerializeField] string _gameScene;
        [SerializeField] AudioSource _audio;

        public void OnStartButtonClicked() {
            OnLoading().Forget();
        }

        public async UniTaskVoid OnLoading() {
            _audio.Play();
            await GameManager.Instance.FadingCanvas
                    .DOFade(1f, 5f)
                    .From(0f, true)
                    .AsyncWaitForCompletion()
                    .AsUniTask();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_gameScene);

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
