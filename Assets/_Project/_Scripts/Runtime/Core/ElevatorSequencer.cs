using UnityEngine;
using PingPingProduction.ProjectAnomaly.Interaction;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

namespace PingPingProduction.ProjectAnomaly.Core {
    public class ElevatorSequencer : MonoBehaviour {
        [Header("Animations")]
        [SerializeField] Animator _yuukiElevatorAnimator;
        [SerializeField] Animator _hinaElevatorAnimator;
        [SerializeField] SequenceMover _yuukiMover;
        [SerializeField] SequenceMover _hinaMover;
        [SerializeField, Range(1f, 10f)] float _elevatorMoveDuration = 1f;
        public float ElevatorMoveDuration => _elevatorMoveDuration;
        [SerializeField, Range(1f, 10f)] int _elevatorOpenCloseDuration = 1;
        public int ElevatorOpenCloseDuration => _elevatorOpenCloseDuration;

        [Header("Audio")]
        [SerializeField] AudioSource _yuukiElevatorAudioSource;
        [SerializeField] AudioSource _hinaElevatorAudioSource;
        [SerializeField] AudioClip _elevatorLoopClip;
        [SerializeField] AudioClip _elevatorOpenClip;
        [SerializeField] AudioClip _elevatorCloseClip;

        public void PlayAnimation(ElevatorType type, string anim) {
            if (type == ElevatorType.Yuuki) {
                _yuukiElevatorAnimator.Play(anim);
            }
            else {
                _hinaElevatorAnimator.Play(anim);
            }
        }

        public void PlayAudio(ElevatorType type, ElevatorAudioClipType clipType) {
            var clip = GetAudioClip(clipType);

            if (type == ElevatorType.Yuuki) {
                _yuukiElevatorAudioSource.PlayOneShot(clip);
            }
            else {
                _hinaElevatorAudioSource.PlayOneShot(clip);
            }
        }

        public void PlayLoopAudio(ElevatorType type, ElevatorAudioClipType clipType) {
            var clip = GetAudioClip(clipType);

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

        public void StopAudio(ElevatorType type) {
            if (type == ElevatorType.Yuuki) {
                _yuukiElevatorAudioSource.loop = false;
                _yuukiElevatorAudioSource.Stop();
            }
            else {
                _hinaElevatorAudioSource.loop = false;
                _hinaElevatorAudioSource.Stop();
            }
        }

        AudioClip GetAudioClip(ElevatorAudioClipType clipType) {
            return clipType switch {
                ElevatorAudioClipType.Open => _elevatorOpenClip,
                ElevatorAudioClipType.Close => _elevatorCloseClip,
                ElevatorAudioClipType.Loop => _elevatorLoopClip,
                _ => null,
            };
        }

        public UniTask[] ProgressMover(bool isMoveForward) {
            var hinaMover = isMoveForward ? _hinaMover.MoveForward() : _hinaMover.MoveToStart();
            var yuukiMover = isMoveForward ? _yuukiMover.MoveForward() : _yuukiMover.MoveToStart();

            return new UniTask[] { hinaMover, yuukiMover }; 
        }

        public enum ElevatorAudioClipType {
            Open, Close, Loop
        }

        public static class ElevatorAnimConst {
            public const string OPEN_ELEVATOR = "anim_elevator_opening";
            public const string CLOSE_ELEVATOR = "anim_elevator_closing";
        }
    }
}
