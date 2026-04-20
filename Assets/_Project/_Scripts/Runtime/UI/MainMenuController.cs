using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PingPingProduction.ProjectAnomaly.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PingPingProduction.ProjectAnomaly.UI {
    public class MainMenuController : MonoBehaviour {
        [SerializeField] string _gameScene;
        [SerializeField] AudioSource _audio;

        [Header("Codex")]
        [SerializeField] Canvas _codexCanvas;
        [SerializeField] GameObject _codexBTN;
        [SerializeField] Canvas _mainMenuCanvas;
        [SerializeField] CodexController _codexController;

        bool _isClicked = false;
        public bool IsEnterCodex;

        void Start() {
            GameManager.Instance.FadingCanvas.DOFade(0f, 2f);

            if (GameManager.Instance.AnomalyFlags.Count <= 0) {
                _codexBTN.SetActive(false);
            }
            else {
                _codexBTN.SetActive(true);
            }

            if (!GameManager.Instance.IsPause)
                GameManager.Instance.Pause();
        }

        public void OnStartButtonClicked() {
            if (IsEnterCodex) return;

            if (!_isClicked)
                OnLoading().Forget();
        }

        public async UniTaskVoid OnLoading() {
            _isClicked = true;

            _audio.Play();
            await GameManager.Instance.FadingCanvas
                    .DOFade(1f, 5f)
                    .From(0f, true)
                    .AsyncWaitForCompletion()
                    .AsUniTask();

            GameManager.Instance.UpdateAnomalyFlag(GameManager.Instance.CardCollectionKeys[0].Key);

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

        public void OnCodexButtonClicked() {
            if (IsEnterCodex) return;

            IsEnterCodex = true;
            _mainMenuCanvas.gameObject.SetActive(false);
            _codexController.StartUp();
        }

        public void OnExitButtonClicked() {
            if (IsEnterCodex) return;

            GameManager.Instance.End();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
