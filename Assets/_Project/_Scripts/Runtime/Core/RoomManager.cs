using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class RoomManager : MonoBehaviour {
        [SerializeField]
        private HallwayDeck _deckSource;

        [SerializeField]
        private TMP_Text _progressTXT;
        [SerializeField]
        private TMP_Text _winTXT;

        private readonly List<RuntimeHallwayConfig> _deck = new();
        private RuntimeHallwayConfig _currentHallway;

        private int _progress;
        private int _indexer = -1;

        private bool _isWin = false;

        private void Awake() {
            InitializeDeck();
            LoadNextHallway();
        }

        private void InitializeDeck() {
            _deck.Clear();

            for (int i = 0; i < _deckSource.Hallways.Count; i++) {
                for (int j = 0; j < _deckSource.Hallways[i].Quantity; j++) {
                    _deck.Add(new(
                        _deckSource.Hallways[i].Config.SceneHolder,
                        _deckSource.Hallways[i].Config.IsAnomaly
                    ));
                }
            }

            for (int i = 0; i < _deck.Count; i++) {
                int rand = Random.Range(i, _deck.Count);
                (_deck[i], _deck[rand]) = (_deck[rand], _deck[i]);
            }

            _progress = 0;
            _indexer = 0;
        }

        public void OnLiftEntered(LiftIdentity liftIdentity) {
            var isCorrect = (_currentHallway.IsAnomaly && GameManager.Instance.LastestRideOnLift == liftIdentity) ||
                            (!_currentHallway.IsAnomaly && GameManager.Instance.LastestRideOnLift != liftIdentity);

            Debug.Log($"Is Correct: {isCorrect}");

            if (isCorrect) {
                _currentHallway.Explore();

                if (_currentHallway.IsAnomaly) {
                    _progress++;
                    _progressTXT.text = $"Progress : {_progress}";
                }

                if (_deck.All(e => e.IsExplored && e.IsAnomaly)) {
                    _isWin = true;
                    _winTXT.gameObject.SetActive(true);
                    return;
                }

                if (!_isWin) {
                    _indexer++;
                    LoadNextHallway();
                }
            }
            else {
                FailRun();
            }
        }

        private void LoadNextHallway() {
            if (_currentHallway != null) {
                SceneManager.UnloadSceneAsync(_currentHallway.SceneName);
            }

            _currentHallway = _deck[_indexer];
            SceneManager.LoadSceneAsync(_currentHallway.SceneName, LoadSceneMode.Additive);
        }

        private void FailRun() {
            if (_currentHallway != null)
                SceneManager.UnloadSceneAsync(_currentHallway.SceneName);

            _progress = 0;
            _indexer = -1;
            _progressTXT.text = $"Progress : {_progress}";

            InitializeDeck();
            LoadNextHallway();
        }
    }
}
