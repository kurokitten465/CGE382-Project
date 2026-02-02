using System;
using System.Collections.Generic;
using UnityEngine;
using PingPingProduction.ProjectAnomaly.Utilities;
using System.Linq;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class GameManager : MonoSingleton<GameManager> {
        // ------------ GameStates ------------ //
        [field: SerializeField]
        public GameState CurremtGameState { get; private set; } = GameState.None;

        public Action OnGameStateChanged;

        public void TransitionState(GameState state) {
            CurremtGameState = state;
            OnGameStateChanged?.Invoke();
        }

        // ------------ Lastest Lift ------------ //
        [field: SerializeField]
        public LiftIdentity LastestRideOnLift { get; private set; }

        public void SetLastestRideOnLift(LiftIdentity liftIdentity) {
            LastestRideOnLift = liftIdentity;
        }
    }

    public enum GameState {
        None,
        Observe,
        Resolve
    }

    public enum LiftIdentity {
        LiftYuuki = 0,
        LiftHina = 1
    }
}
