using Sonar;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "IntGameEvent", menuName = "Sonar/Events/Int Game Event")]
public class IntEventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform when the event is raised.")]
    public UnityAction<int> OnEventRaised;
    public void RaiseEvent(int value)
    {
        OnEventRaised?.Invoke(value);
    }
}