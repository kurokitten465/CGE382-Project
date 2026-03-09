using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PingPingProduction.ProjectAnomaly {
    public class CustomFillHighlightButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] Image _fillImage;
        [SerializeField, Range(0.1f, 2f)] float _duration = 0.1f;

        public void OnPointerEnter(PointerEventData eventData) {
            _fillImage.DOFillAmount(1f, _duration);
        }

        public void OnPointerExit(PointerEventData eventData) {
            _fillImage.DOFillAmount(0f, _duration);
        }
    }
}
