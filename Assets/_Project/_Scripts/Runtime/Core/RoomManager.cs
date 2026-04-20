using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PingPingProduction.ProjectAnomaly.Interaction;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class RoomManager : MonoBehaviour {
        [Header("Settings")]
        [SerializeField, Range(0f, 1f)] float _anomalyChance = 0.6f;
        [SerializeField] Transform _hallwayTopPoint;
        [SerializeField] Transform _hallwayBelowpoint;

        [Header("Depemdencies")]
        [SerializeField] HallwayRegistry _hallwayRegistry;
        [SerializeField] ElevatorSequencer _elevatorSequencer;

        public HallwayConfig CurrentHallway { get; private set; }
        public HallwayConfig PreviousHallway { get; private set; }

        const byte MAX_ANOMALY_ATTEMPTS = 4;
        byte _minAnomalyAttempts = 0;
        byte _curremtAnomalyAttempts = 0;
        byte _currentProgress = 0;

        readonly HashSet<byte> _lastAnomalyIndex = new();
        GameObject _currentHallwayGO;
        GameObject _previousHallwayGO;

        public GameObject Generate(int index) {
            if (index < 0 || index >= _hallwayRegistry.Hallways.Count) return null;

            PreviousHallway = CurrentHallway;
            CurrentHallway = _hallwayRegistry.Hallways[index];
            _minAnomalyAttempts = (byte)Random.Range(1, MAX_ANOMALY_ATTEMPTS + 1);
            if (_currentHallwayGO != null) Destroy(_currentHallwayGO);

            var obj = Instantiate(CurrentHallway.HallwayPrefab, Vector3.zero, Quaternion.identity);
            _currentHallwayGO = obj;

            _elevatorSequencer.PlayAudio(ElevatorType.Hina, ElevatorSequencer.ElevatorAudioClipType.Open);
            _elevatorSequencer.PlayAnimation(ElevatorType.Yuuki, ElevatorSequencer.ElevatorAnimConst.OPEN_ELEVATOR);
            _elevatorSequencer.PlayAnimation(ElevatorType.Hina, ElevatorSequencer.ElevatorAnimConst.OPEN_ELEVATOR);

            return obj;
        }

        public async UniTask<GameObject> GenerateAsync(ElevatorButtonTrigger buttonTrigger, bool genDefault = false) {
            if (genDefault)
                _lastAnomalyIndex.Clear();

            PreviousHallway = CurrentHallway;
            CurrentHallway = genDefault ? _hallwayRegistry.Hallways[0] : RandomHallway();

            var buttonDirection = buttonTrigger.ElevatorDirection;

            var spawnPoint = buttonDirection == ElevatorButtonDirection.Upward ? _hallwayTopPoint.position : _hallwayBelowpoint.position;

            var go = Instantiate(CurrentHallway.HallwayPrefab, spawnPoint, Quaternion.identity);
            _previousHallwayGO = _currentHallwayGO;
            _currentHallwayGO = go;

            if (buttonDirection == ElevatorButtonDirection.Upward) {
                await ElevetorMoveHandler(_hallwayBelowpoint.position.y, buttonTrigger);
            }
            else {
                await ElevetorMoveHandler(_hallwayTopPoint.position.y, buttonTrigger);
            }

            return go;
        }

        HallwayConfig RandomHallway() {
            float roll = Random.value;

            // 60% normal room
            if (roll > _anomalyChance && _curremtAnomalyAttempts <= _minAnomalyAttempts) {
                _curremtAnomalyAttempts++;
                return _hallwayRegistry.Hallways[0];
            }

            // anomaly selection
            byte anomalyCount = (byte)(_hallwayRegistry.Hallways.Count - 1);

            if (anomalyCount <= 0)
                return _hallwayRegistry.Hallways[0];

            byte randomIndex;

            do
                randomIndex = (byte)Random.Range(1, _hallwayRegistry.Hallways.Count);
            while (_lastAnomalyIndex.Contains(randomIndex) && anomalyCount > 1);

            _lastAnomalyIndex.Add(randomIndex);
            _curremtAnomalyAttempts = 0;
            _minAnomalyAttempts = (byte)Random.Range(1, MAX_ANOMALY_ATTEMPTS + 1);

            return _hallwayRegistry.Hallways[randomIndex];
        }

        async UniTask ElevetorMoveHandler(float yVal, ElevatorButtonTrigger buttonTrigger) {
            _elevatorSequencer.PlayAnimation(ElevatorType.Yuuki, ElevatorSequencer.ElevatorAnimConst.CLOSE_ELEVATOR);
            _elevatorSequencer.PlayAnimation(ElevatorType.Hina, ElevatorSequencer.ElevatorAnimConst.CLOSE_ELEVATOR);

            var elevatorType = buttonTrigger.ElevatorTrigger.Elevator;

            _elevatorSequencer.PlayAudio(elevatorType, ElevatorSequencer.ElevatorAudioClipType.Close);

            await UniTask.Delay(_elevatorSequencer.ElevatorOpenCloseDuration * 1000);

            var previousHallwayTask = _previousHallwayGO.transform
                    .DOMoveY(yVal, _elevatorSequencer.ElevatorMoveDuration)
                    .AsyncWaitForCompletion()
                    .AsUniTask();

            var currentHallwayTask = _currentHallwayGO.transform
                    .DOMoveY(0f, _elevatorSequencer.ElevatorMoveDuration)
                    .AsyncWaitForCompletion()
                    .AsUniTask();
            
            var progressMovers = new UniTask[2]; 

            if (ProgressManager.AnomalyFounded != _currentProgress) {
                if (ProgressManager.AnomalyFounded < _currentProgress) {
                    progressMovers = _elevatorSequencer.ProgressMover(false);
                }
                else if (ProgressManager.AnomalyFounded > _currentProgress) {
                    progressMovers = _elevatorSequencer.ProgressMover(true);
                }

                _currentProgress = ProgressManager.AnomalyFounded;
            }

            _elevatorSequencer.PlayLoopAudio(elevatorType, ElevatorSequencer.ElevatorAudioClipType.Loop);

            await UniTask.WhenAll(previousHallwayTask, currentHallwayTask, progressMovers[0], progressMovers[1]);

            Destroy(_previousHallwayGO);

            _elevatorSequencer.StopAudio(elevatorType);

            _elevatorSequencer.PlayAudio(elevatorType, ElevatorSequencer.ElevatorAudioClipType.Open);
            _elevatorSequencer.PlayAnimation(ElevatorType.Yuuki, ElevatorSequencer.ElevatorAnimConst.OPEN_ELEVATOR);
            _elevatorSequencer.PlayAnimation(ElevatorType.Hina, ElevatorSequencer.ElevatorAnimConst.OPEN_ELEVATOR);
        }
    }
}

