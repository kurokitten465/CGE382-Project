using System;
using UnityEngine;
using PingPingProduction.ProjectAnomaly.Core.Input;
using PingPingProduction.ProjectAnomaly.Utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using PingPingProduction.ProjectAnomaly.Interaction;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class GameManager : MonoSingleton<GameManager> {
        [Header("Fading")]
        [field: SerializeField] public CanvasGroup FadingCanvas { get; private set; }
        [field: SerializeField] public TMP_Text WinText { get; private set; }
        [field: SerializeField] public TMP_Text LostText { get; private set; }

        [Header("Dependencies")]
        [SerializeField] InputReader _inputReader;

        [Header("Startup")]
        [SerializeField] bool _useStartup;
        [SerializeField] string _startupScene;

        [field: SerializeField] public CardCollectionKey[] CardCollectionKeys { get; private set; }

        // Exposing Member
        public bool IsPause { get; private set; } = false;
        public readonly HashSet<string> AnomalyFlags = new();
        public bool IsBossRoom = false;
        public ElevatorType PlayerLastElevator;

        // Init
        protected override void Awake() {
            base.Awake();
            Pause();
            if (_useStartup)
                OnLoading().Forget();

            LostText.alpha = 0f;
            WinText.alpha = 0f;
        }

        // GamePaused
        public static Action<bool> OnGamePaused;
        public bool Pause() {
            IsPause = !IsPause;

            if (IsPause) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _inputReader.SwitchMapTo(InputReader.ActionMap.UI);
            }
            else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _inputReader.SwitchMapTo(InputReader.ActionMap.Player);
            }

            OnGamePaused?.Invoke(IsPause);

            return IsPause;
        }

        public bool Pause(bool set) {
            IsPause = set;

            if (IsPause) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _inputReader.SwitchMapTo(InputReader.ActionMap.UI);
            }
            else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _inputReader.SwitchMapTo(InputReader.ActionMap.Player);
            }

            OnGamePaused?.Invoke(IsPause);

            return IsPause;
        }

        public bool PauseWithOutNotify(bool set) {
            IsPause = set;

            if (IsPause) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _inputReader.SwitchMapTo(InputReader.ActionMap.UI);
            }
            else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _inputReader.SwitchMapTo(InputReader.ActionMap.Player);
            }

            return IsPause;
        }

        // GameEnded
        public static Action OnGameEnded;
        public void End() {
            OnGameEnded?.Invoke();
        }

        // GameWin
        public static Action OnGameWin;
        public void Win() {
            OnGameWin?.Invoke();
            UpdateAnomalyFlag(CardCollectionKeys.Last().Key);
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

        // Save Progress
        public void UpdateAnomalyFlag(string key) {
            foreach (var e in CardCollectionKeys) {
                if (key == e.Key) {
                    AnomalyFlags.Add(e.Key);
                }
            }
        }

        public void UpdateAnomalyFlag(HallwayConfig config) {
            foreach (var e in CardCollectionKeys) {
                if (config.HallwayPrefab.name == e.Key) {
                    AnomalyFlags.Add(e.Key);
                }
            }
        }

        void OnGUI() {
            
        }
    }

    [Serializable]
    public class CardCollectionKey
    {
        public string Key;
        public HallwayConfig Config;
    }
}
