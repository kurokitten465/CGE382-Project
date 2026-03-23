using UnityEngine;

namespace PingPingProduction.ProjectAnomaly.Interaction
{
    public class MainMenuInteraction : MonoBehaviour
    {
        [Header("Window 1-2-4")]
        [SerializeField] Animator _windowAnimator_1;
        [SerializeField] Animator _windowAnimator_2;
        [SerializeField] Animator _windowAnimator_4;

        bool _isWindow_1_Opened;
        bool _isWindow_2_Opened;
        bool _isWindow_4_Opened;

        [Header("Stair Interaction")]
        [SerializeField] AudioSource _stairSource;
        [SerializeField] AudioClip[] _stairClips;

        [Header("Door Interaction")]
        [SerializeField] Animator _doorAnimator;

        public void OnWindow_1_Interacted(MainMenuTrigger trigger) {
            if (_isWindow_1_Opened)
                _windowAnimator_1.Play("anim_window_2_close");
            else
                _windowAnimator_1.Play("anim_window_2_open");

            _isWindow_1_Opened = !_isWindow_1_Opened;

            trigger.OnPointedAway();
        }

        public void OnWindow_2_Interacted(MainMenuTrigger trigger) {
            if (_isWindow_2_Opened)
                _windowAnimator_2.Play("anim_window_1_close");
            else
                _windowAnimator_2.Play("anim_window_1_open");

            _isWindow_2_Opened = !_isWindow_2_Opened;

            trigger.OnPointedAway();
        }

        public void OnWindow_3_Interacted(MainMenuTrigger trigger) {
            
        }

        public void OnWindow_4_Interacted(MainMenuTrigger trigger) {
            if (_isWindow_4_Opened)
                _windowAnimator_4.Play("anim_window_4_close");
            else
                _windowAnimator_4.Play("anim_window_4_open");

            _isWindow_4_Opened = !_isWindow_4_Opened;

            trigger.OnPointedAway();
        }

        public void OnWindow_5_Interacted(MainMenuTrigger trigger) {
            
        }

        public void OnStair_Interacted(MainMenuTrigger trigger) {
            _stairSource.PlayOneShot(_stairClips[Random.Range(0, _stairClips.Length)]);
            trigger.OnPointedAway();
        }

        public void OnDoor_Interacted(MainMenuTrigger trigger) {
            
        }
    }
}
