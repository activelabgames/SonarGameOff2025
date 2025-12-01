using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace Sonar
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClipInformationEventChannelSO audioClipInformationEvent;

        [SerializeField] private Vector3EventChannelSO playerMoveEvent;

        [SerializeField] private AudioClip currentAudioClip;

        [SerializeField] private AudioSource audioSource;
        private AudioFaderCurve audioFaderCurve;

        [SerializeField] AudioListener audioListener;

        private Vector3 playerPosition = Vector3.zero;

        private void Awake()
        {
            audioListener = GetComponent<AudioListener>();
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            audioFaderCurve = audioSource.GetComponent<AudioFaderCurve>();
            audioSource.ignoreListenerPause = true;
        }

        private void OnEnable()
        {
            if (audioClipInformationEvent != null)
            {
                audioClipInformationEvent.OnEventRaised += HandleAudioClipInformationEvent;
            }
            if (playerMoveEvent != null)
            {
                playerMoveEvent.OnEventRaised += HandlePlayerMoveEvent;
            }
            
        }

        private void OnDisable()
        {
            if (audioClipInformationEvent != null)
            {
                audioClipInformationEvent.OnEventRaised -= HandleAudioClipInformationEvent;
            }

            if (playerMoveEvent != null)
            {
                playerMoveEvent.OnEventRaised -= HandlePlayerMoveEvent;
            }
        }

        private void Update()
        {
            transform.position = this.playerPosition;
        }
        
        private void HandlePlayerMoveEvent(Vector3 playerPosition)
        {
            this.playerPosition = playerPosition;
        }

        public void SetAudioListenerEnabled(bool enabled)
        {
            audioListener.enabled = enabled;
        }

        private void HandleAudioClipInformationEvent(AudioClipInformation audioClipInformation)
        {
            if (audioClipInformation == null)
            {
                Debug.Log("AudioClipInformation is null.");
                audioFaderCurve.FadeIn();
                return;
            }
            currentAudioClip = audioClipInformation.AudioClip;
            audioSource.resource = currentAudioClip;
            audioSource.volume = audioClipInformation.Volume;
            audioSource.Play();
        }
    }
}
