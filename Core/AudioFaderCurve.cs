using UnityEngine;
using System.Collections;

namespace Sonar
{
    public class AudioFaderCurve : MonoBehaviour
    {
        public AudioSource audioSource;
        public AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1); // Default linear curve
        public float fadeTime = 1.0f; // Time in seconds to fade in/out

        // Ensure an AudioSource is present
        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("No AudioSource found on this GameObject!");
                enabled = false; // Disable the script to prevent further errors
            }
        }

        public void FadeIn()
        {
            StartCoroutine(Fade(0f, 1f, fadeTime));
        }

        public void FadeOut()
        {
            StartCoroutine(Fade(1f, 0f, fadeTime));
        }

        private IEnumerator Fade(float startVolume, float endVolume, float duration)
        {
            if (audioSource == null) yield break; // Exit if no AudioSource

            float currentTime = 0;
            float initialVolume = audioSource.volume;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float timeRatio = currentTime / duration;
                audioSource.volume = Mathf.Lerp(startVolume, endVolume, fadeCurve.Evaluate(timeRatio));
                yield return null;
            }

            // Ensure the volume is set to the final value
            audioSource.volume = endVolume;

            // Stop the audio if fading out completely
            if (endVolume == 0)
            {
                audioSource.Stop();
            }
        }
    }    
}
