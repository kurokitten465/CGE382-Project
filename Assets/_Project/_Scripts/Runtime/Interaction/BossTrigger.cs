using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PingPingProduction.ProjectAnomaly.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PingPingProduction.ProjectAnomaly {
    public class BossTrigger : MonoBehaviour {
        [Header("Collision")]
        public string playerTag = "Player";

        void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag(playerTag)) {
                OnPlayerLost().Forget();
            }
        }

        async UniTask OnPlayerLost() {
            GameManager.Instance.PauseWithOutNotify(true);
            GameManager.Instance.FadingCanvas.alpha = 1f;
            
            await GameManager.Instance.FadingCanvas.DOFade(1f, 2f).From(0f, true).AsyncWaitForCompletion().AsUniTask();
            await GameManager.Instance.LostText.DOFade(1f, 1f).From(0f, true).AsyncWaitForCompletion().AsUniTask();

            await Task.Delay(8000);

            await GameManager.Instance.LostText.DOFade(0f, 1f).From(1f, true).AsyncWaitForCompletion().AsUniTask();

            ProgressManager.IsResolving = false;

            OnLoading().Forget();
        }

        public async UniTaskVoid OnLoading() {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("scene_main_menu");

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
    }
}
