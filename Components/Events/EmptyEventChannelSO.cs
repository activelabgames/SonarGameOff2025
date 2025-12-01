using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EmptyGameEvent", menuName = "Sonar/Events/Empty Game Event")]
public class EmptyEventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform when the event is raised.")]
    public UnityAction OnEventRaised;
    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}