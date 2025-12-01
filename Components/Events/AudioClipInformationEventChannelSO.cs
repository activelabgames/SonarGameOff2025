using UnityEngine;
using UnityEngine.Events;

namespace Sonar
{
    [CreateAssetMenu(fileName = "AudioClipInformationGameEvent", menuName = "Sonar/Events/AudioClipInformation Game Event")]
    public class AudioClipInformationEventChannelSO : ScriptableObject
    {
        [Tooltip("The action to perform when the event is raised.")]
        public UnityAction<AudioClipInformation> OnEventRaised;
        public void RaiseEvent(AudioClipInformation value)
        {
            OnEventRaised?.Invoke(value);
        }
    }    
}
