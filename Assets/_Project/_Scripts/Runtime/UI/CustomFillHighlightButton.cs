using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PingPingProduction.ProjectAnomaly {
    public class CustomFillHighlightButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] Image _fillImage;
        [SerializeField, Range(0.1f, 2f)] float _duration = 0.1f;
        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip _emterClip;
        [SerializeField] AudioClip _exitClip;

        public void OnPointerEnter(PointerEventData eventData) {
            _fillImage.DOFillAmount(1f, _duration);
            _audioSource.PlayOneShot(_emterClip);
        }

        public void OnPointerExit(PointerEventData eventData) {
            _fillImage.DOFillAmount(0f, _duration);
        }
    }
}
