using Sonar;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "StringGameEvent", menuName = "Sonar/Events/String Game Event")]
public class StringEventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform when the event is raised.")]
    public UnityAction<string> OnEventRaised;
    public void RaiseEvent(string value)
    {
        OnEventRaised?.Invoke(value);
    }
}