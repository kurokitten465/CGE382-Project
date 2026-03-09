using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using PingPingProduction.ProjectAnomaly.Interaction;
using DG.Tweening;
using System.Linq;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class RoomManager : MonoBehaviour {
        [Header("Settings")]
        [SerializeField, Range(0f, 1f)] float _anomalyChance = 0.4f;
        [SerializeField] Transform _hallwayTopPoint;
        [SerializeField] Transform _hallwayBelowpoint;

        [Header("Depemdencies")]
        [SerializeField] HallwayRegistry _hallwayRegistry;

        [Header("Animations")]
        [SerializeField] Animator _yuukiElevatorAnimator;
        [SerializeField] Animator _hinaElevatorAnimator;
        [SerializeField] string _elevatorDoorOpening;
        [SerializeField] string _elevatorDoorClosing;
        [SerializeField, Range(1f, 10f)] float _elevatorDuration = 1f;
        [SerializeField, Range(1f, 10f)] int _elevatorOpemCLoseDuration = 1;

        [Header("Audio")]
        [SerializeField] AudioSource _yuukiElevatorAudioSource;
        [SerializeField] AudioSource _hinaElevatorAudioSource;
        [SerializeField] AudioClip _elevatorLoopClip;
        [SerializeField] AudioClip _elevatorOpenClip;
        [SerializeField] AudioClip _elevatorCloseClip;

        public HallwayConfig CurrentHallway { get; private set; }
        public HallwayConfig PreviousHallway { get; private set; }

        Animator _yuukiDoorAnimator;
        Animator _hinaDoorAnimator;

        readonly HashSet<byte> _lastAnomalyIndex = new();
        GameObject _currentHallwayGO;
        GameObject _previousHallwayGO;

#if UNITY_EDITOR
        [ContextMenu("Generate Hallway")]
        public GameObject Generate() {
            PreviousHallway = CurrentHallway;
            CurrentHallway = RandomHallway();
            if (_currentHallwayGO != null) Destroy(_currentHallwayGO);

            var go = Instantiate(CurrentHallway.HallwayPrefab, Vector3.zero, Quaternion.identity);
            _currentHallwayGO = go;
            return go;
        }
#endif

        public GameObject Generate(int index) {
            if (index < 0 || index >= _hallwayRegistry.Hallways.Count) return null;

            PreviousHallway = CurrentHallway;
            CurrentHallway = _hallwayRegistry.Hallways[index];
            if (_currentHallwayGO != null) Destroy(_currentHallwayGO);

            var obj = Instantiate(CurrentHallway.HallwayPrefab, Vector3.zero, Quaternion.identity);
            _currentHallwayGO = obj;

            PlayAudio(ElevatorType.Hina, _elevatorOpenClip);
            _yuukiElevatorAnimator.Play(_elevatorDoorOpening);
            _hinaElevatorAnimator.Play(_elevatorDoorOpening);

            return obj;
        }

        public async UniTask<GameObject> GenerateAsync(ElevatorButtonTrigger buttonTrigger, bool genDefault = false) {
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
            if (roll > _anomalyChance)
                return _hallwayRegistry.Hallways[0];

            // anomaly selection
            byte anomalyCount = (byte)(_hallwayRegistry.Hallways.Count - 1);

            if (anomalyCount <= 0)
                return _hallwayRegistry.Hallways[0];

            byte randomIndex;

            do
                randomIndex = (byte)Random.Range(1, _hallwayRegistry.Hallways.Count);
            while (_lastAnomalyIndex.Contains(randomIndex) && anomalyCount > 1);

            _lastAnomalyIndex.Add(randomIndex);

            return _hallwayRegistry.Hallways[randomIndex];
        }

        async UniTask ElevetorMoveHandler(float yVal, ElevatorButtonTrigger buttonTrigger) {
            _yuukiElevatorAnimator.Play(_elevatorDoorClosing);
            _hinaElevatorAnimator.Play(_elevatorDoorClosing);

            var elevatorType = buttonTrigger.ElevatorTrigger.Elevator;

            PlayAudio(elevatorType, _elevatorCloseClip);

            await UniTask.Delay(_elevatorOpemCLoseDuration * 1000);

            var previousHallwayTask = _previousHallwayGO.transform
                    .DOMoveY(yVal, _elevatorDuration)
                    .AsyncWaitForCompletion()
                    .AsUniTask();

            var currentHallwayTask = _currentHallwayGO.transform
                    .DOMoveY(0f, _elevatorDuration)
                    .AsyncWaitForCompletion()
                    .AsUniTask();

            PlayLoopAudio(elevatorType, _elevatorLoopClip);

            await UniTask.WhenAll(previousHallwayTask, currentHallwayTask);

            Destroy(_previousHallwayGO);

            StopAudio(elevatorType);

            PlayAudio(elevatorType, _elevatorOpenClip);
            _yuukiElevatorAnimator.Play(_elevatorDoorOpening);
            _hinaElevatorAnimator.Play(_elevatorDoorOpening);
        }

        void PlayAudio(ElevatorType type, AudioClip clip) {
            if (type == ElevatorType.Yuuki) {
                _yuukiElevatorAudioSource.PlayOneShot(clip);
            }
            else {
                _hinaElevatorAudioSource.PlayOneShot(clip);
            }
        }

        void PlayLoopAudio(ElevatorType type, AudioClip clip) {

            if (type == ElevatorType.Yuuki) {
                _yuukiElevatorAudioSource.loop = true;
                _yuukiElevatorAudioSource.clip = clip;
                _yuukiElevatorAudioSource.Play();
            }
            else {
                _hinaElevatorAudioSource.loop = true;
                _hinaElevatorAudioSource.clip = clip;
                _hinaElevatorAudioSource.Play();
            }
        }

        void StopAudio(ElevatorType type) {
            if (type == ElevatorType.Yuuki) {
                _yuukiElevatorAudioSource.loop = false;
                _yuukiElevatorAudioSource.Stop();
            }
            else {
                _hinaElevatorAudioSource.loop = false;
                _hinaElevatorAudioSource.Stop();
            }
        }
    }
}

