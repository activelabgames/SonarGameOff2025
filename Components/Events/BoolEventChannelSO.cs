using Sonar;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "BoolGameEvent", menuName = "Sonar/Events/Bool Game Event")]
public class BoolEventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform when the event is raised.")]
    public UnityAction<bool> OnEventRaised;
    public void RaiseEvent(bool value)
    {
        OnEventRaised?.Invoke(value);
    }
}