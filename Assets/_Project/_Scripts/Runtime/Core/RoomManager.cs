using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class RoomManager : MonoBehaviour {
        [Header("Scene Tranforms")]
        [SerializeField] private Transform _sceneInitPos;
        [SerializeField] private Transform _sceneEndedPos;

        [Header("Deck Config")]
        [SerializeField]
        private HallwayDeck _deckSource;
        [SerializeField]
        private HallwayConfig _starterRoomConfig;
        private RuntimeHallwayConfig _starterRoomRuntimeConfig;

        private Queue<RuntimeHallwayConfig> _currentDeck;
        private readonly List<RuntimeHallwayConfig> _hallwayPools = new();

        private RuntimeHallwayConfig _currentRuntimeHallwayConfig;
        private GameObject _currentHallwayObject;
        private RuntimeHallwayConfig _oldRuntimeHallwayConfig;
        private GameObject _oldHallwayObject;

        private void Awake() {
            InitializeDeck();
            ShuffleDeck();
            InstantiateStarter();
        }

        private void InitializeDeck() {
            _starterRoomRuntimeConfig = new(_starterRoomConfig.SceneHolder, _starterRoomConfig.IsAnomaly);

            foreach (var hallwayConfig in _deckSource.Hallways) {
                for (int i = 0; i < hallwayConfig.Quantity; i++) {
                    _hallwayPools.Add(new(hallwayConfig.Config.SceneHolder, hallwayConfig.Config.IsAnomaly));
                }
            }

            for (int i = 0; i < _hallwayPools.Count; i++) {
                if (_hallwayPools[i].IsAnomaly)
                    GameManager.Instance.MaxAnomalyLevel++;
            }
        }

        private void ShuffleDeck() {
            for (int i = 0; i < _hallwayPools.Count; i++) {
                int rand = UnityEngine.Random.Range(i, _hallwayPools.Count);

                (_hallwayPools[i], _hallwayPools[rand]) = (_hallwayPools[rand], _hallwayPools[i]);
            }

            _currentDeck = new(_hallwayPools);
        }

        private async UniTask LoadNextHallway() {
            _oldHallwayObject = _currentHallwayObject;
            _oldRuntimeHallwayConfig = _currentRuntimeHallwayConfig;

            _currentRuntimeHallwayConfig = _currentDeck.Dequeue();

            _currentHallwayObject = (await InstantiateAsync(_currentRuntimeHallwayConfig.HallwayPrefab, _sceneInitPos.position, Quaternion.identity).ToUniTask())[0];

            _oldHallwayObject.transform.DOLocalMoveY(_sceneEndedPos.transform.position.y, 3f);
            _currentHallwayObject.transform.DOLocalMoveY(0, 3f);

            await UniTask.Delay(TimeSpan.FromSeconds(3), ignoreTimeScale: false);

            Destroy(_oldHallwayObject);
        }

        private void InstantiateStarter() {
            _currentRuntimeHallwayConfig = _starterRoomRuntimeConfig;
            _currentHallwayObject = Instantiate(_starterRoomRuntimeConfig.HallwayPrefab, Vector3.zero, Quaternion.identity);
        }

        public void OnLiftEntered(LiftIdentity liftIdentity) {
            if (GameManager.Instance.IsCorrectAnswer(liftIdentity, _currentRuntimeHallwayConfig)) {
                if (GameManager.Instance.IsWin()) {
                    Debug.Log($"Win");
                    return;
                }

                LoadNextHallway().Forget();
            }
            else {
                ShuffleDeck();
                LoadNextHallway().Forget();
            }
        }
    }
}
