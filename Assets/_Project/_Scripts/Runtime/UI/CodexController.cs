using System.Linq;
using PingPingProduction.ProjectAnomaly.Core;
using PingPingProduction.ProjectAnomaly.Core.Input;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PingPingProduction.ProjectAnomaly.UI {
    public class CodexController : MonoBehaviour {
        [SerializeField] Canvas _mainMenuCanvas;
        [SerializeField] MainMenuController _mainMenuController;
        [SerializeField] Canvas _codexCanvas;
        [SerializeField] GameObject _cardSpawner;
        [SerializeField] TMP_Text _titleText;
        [SerializeField] TMP_Text _descText;
        [SerializeField] AnomalyCardRegister[] _anomalyCardRegister;
        [SerializeField] GameObject _infoPanel;
        [SerializeField] GameObject _listPanel;

        [Header("Input")]
        //public InputActionReference _acceptAction;
        public InputReader _inputReader;

        bool _isOnInfo;
        bool _isOnList;

        GameObject _currentCardOBJ;

        public void StartUp() {
            if (GameManager.Instance.AnomalyFlags.Count <= 0) return;

            foreach (var card in _anomalyCardRegister) {
                if (GameManager.Instance.AnomalyFlags.Contains(card.ID)) {
                    card.gameObject.SetActive(true);
                }
                else {
                    card.gameObject.SetActive(false);
                }
            }

            _codexCanvas.gameObject.SetActive(true);
            _listPanel.SetActive(true);
            _isOnList = true;
            _inputReader.OnUIRightClicked += OnCanceled;
        }

        public void OnCodexClicked(string id) {
            var cardRegister = _anomalyCardRegister.FirstOrDefault(e => e.ID == id);

            if (cardRegister == null) return;

            if (!GameManager.Instance.AnomalyFlags.Contains(cardRegister.ID)) return;

            _listPanel.SetActive(false);
            _currentCardOBJ = Instantiate(cardRegister.Card, _cardSpawner.transform);
            _titleText.text = cardRegister.Title;
            _descText.text = cardRegister.Desc;

            _infoPanel.SetActive(true);
            _isOnList = false;
            _isOnInfo = true;
        }

        public void OnExitInfoPanel() {
            Destroy(_currentCardOBJ);

            _infoPanel.SetActive(false);
            _isOnInfo = false;
            _isOnList = true;

            _listPanel.SetActive(true);
        }

        public void OnExitListPanel() {
            _listPanel.SetActive(false);
            _mainMenuCanvas.gameObject.SetActive(true);
            _mainMenuController.IsEnterCodex = false;

            _isOnList = false;
            _isOnInfo = false;
            _inputReader.OnUIRightClicked -= OnCanceled;
        }

        void OnCanceled(InputAction.CallbackContext context) {
            Debug.Log($"{context.phase}");

            if (context.phase != InputActionPhase.Started) return;

            if (_isOnList) {
                OnExitListPanel();

                return;
            }

            if (_isOnInfo) {
                OnExitInfoPanel();

                return;
            }
        }
    }
}
