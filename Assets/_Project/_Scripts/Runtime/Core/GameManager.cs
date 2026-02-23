using UnityEngine;
using PingPingProduction.ProjectAnomaly.Utilities;
using System;
using UnityEngine.SceneManagement;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class GameManager : MonoSingleton<GameManager> {
        [Header("Startup")]
        [SerializeField] bool _usedStartUp;
        [SerializeField] string _mainMenuScene;

        void Start() {
            SceneManager.LoadScene(_mainMenuScene);
        }

        public bool IsGamePausing { get; private set; } = false;
        public Action<IPauseContext, bool> OnGamePaused;
        public void Pause(IPauseContext context) {
            var isPaused = IsGamePausing = !IsGamePausing;
            OnGamePaused?.Invoke(context, isPaused);
        }

        public Action OnGameRestarting;
        public void Restart() => OnGameRestarting?.Invoke();

        public Action<GameState> OnGameStateChanged;
        public GameState CurrentGameState { get; private set; }
        public void SetGameState(GameState state) {
            CurrentGameState = state;
            OnGameStateChanged?.Invoke(state);
        }
    }

    public interface IPauseContext { }

    public enum GameState {
        Resolving, Exploring
    }
}
