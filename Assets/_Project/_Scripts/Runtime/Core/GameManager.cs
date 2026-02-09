using System;
using System.Collections.Generic;
using UnityEngine;
using PingPingProduction.ProjectAnomaly.Utilities;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class GameManager : MonoSingleton<GameManager> {
        // ------------ GameStates ------------ //
        [field: SerializeField]
        public GameState CurremtGameState { get; private set; } = GameState.None;

        public Action OnGameStateChanged;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransitionState(GameState state) {
            CurremtGameState = state;
            OnGameStateChanged?.Invoke();
        }

        // ------------ Lastest Lift ------------ //
        [field: SerializeField]
        public LiftIdentity LastestRideOnLift { get; private set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetLastestRideOnLift(LiftIdentity liftIdentity) {
            LastestRideOnLift = liftIdentity;
        }

        // ------------ Progress Level ------------ //
        public byte RoomIndex = 0;
        public byte CurrentAnomalyLevel = 0;
        public byte MaxAnomalyLevel = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsWin() {
            return CurrentAnomalyLevel >= MaxAnomalyLevel;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCorrectAnswer(LiftIdentity liftIdentity, RuntimeHallwayConfig currentHallway) {
            var isCorrect =
            (currentHallway.IsAnomaly && liftIdentity == LastestRideOnLift) ||
            (!currentHallway.IsAnomaly && liftIdentity != LastestRideOnLift);

            if (isCorrect) {
                if (currentHallway.IsAnomaly) {
                    CurrentAnomalyLevel++;
                }

                RoomIndex++;

                return true;
            }
            else {
                CurrentAnomalyLevel = 0;
                RoomIndex = 0;
                return false;
            }
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
