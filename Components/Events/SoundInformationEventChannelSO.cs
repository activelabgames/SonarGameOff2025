using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SoundInformationGameEvent", menuName = "Sonar/Events/SoundInformation Game Event")]
public class SoundInformationEventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform when the event is raised.")]
    public UnityAction<SoundInformation> OnEventRaised;
    public void RaiseEvent(SoundInformation soundInformation)
    {
        Debug.Log($"{soundInformation.Source} emitted sound eventat intensity {soundInformation.Intensity}");
        OnEventRaised?.Invoke(soundInformation);
    }
}