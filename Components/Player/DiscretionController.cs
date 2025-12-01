using UnityEngine;
using UnityEngine.UI;

namespace Sonar
{
    public class DiscretionController : MonoBehaviour
    {

        [SerializeField] private Image discretionBar;
        [SerializeField] private Image[] discretionPoints;
        float maxSoundIntensity = 100f;
        float soundIntensity = 100f;

        [SerializeField] private SoundInformationEventChannelSO engineSoundEvent;
        [SerializeField] private SoundInformationEventChannelSO activeSonarSoundEvent;

        float lerpSpeed;

        private void OnEnable()
        {
            engineSoundEvent.OnEventRaised += HandleSoundEvent;
            activeSonarSoundEvent.OnEventRaised += HandleSoundEvent;
        }

        private void OnDisable()
        {
            engineSoundEvent.OnEventRaised -= HandleSoundEvent;
            activeSonarSoundEvent.OnEventRaised -= HandleSoundEvent;
        }

        public void SetDiscretionBar(Image discretionBar)
        {
            this.discretionBar = discretionBar;
        }
        
        public void SetDiscretionPoints(Image[] discretionPoints)
        {
            this.discretionPoints = discretionPoints;
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            soundIntensity = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log($"Discretion: current sound intensity is {soundIntensity}");
            if (soundIntensity > maxSoundIntensity) soundIntensity = maxSoundIntensity;

            lerpSpeed = 3f * Time.deltaTime;

            DiscretionBarFiller();
        }

        private bool DisplayDiscretionPoints(float soundIntensity, int pointNumber)
        {
            return ((pointNumber * 10) >= soundIntensity);
        }

        private void DiscretionBarFiller()
        {
            if (discretionBar == null)
            {
                return;
            }
            discretionBar.fillAmount = Mathf.Lerp(discretionBar.fillAmount, (soundIntensity / maxSoundIntensity), lerpSpeed);

            for (int i = 0; i < discretionPoints.Length; i++)
            {
                discretionPoints[i].enabled = !DisplayDiscretionPoints(soundIntensity, i);
            }
        }
        
        private void HandleSoundEvent(SoundInformation soundInformation)
        {
            if (soundInformation.Source.gameObject != gameObject)
            {
                Debug.Log($"Discretion: ignoring sound event from another object: {soundInformation.Source.name}");
                return;
            }
            Debug.Log($"Discretion: new sound received from {soundInformation.Source.name}: intensity = {soundInformation.Intensity}");
            //soundIntensity = Mathf.Max(soundIntensity, soundInformation.Intensity);
            soundIntensity = soundInformation.Intensity;
        }
    }
}
