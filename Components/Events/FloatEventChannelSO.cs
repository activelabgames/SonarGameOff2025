using Sonar;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "FloatGameEvent", menuName = "Sonar/Events/Float Game Event")]
public class FloatEventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform when the event is raised.")]
    public UnityAction<float> OnEventRaised;
    public void RaiseEvent(float value)
    {
        OnEventRaised?.Invoke(value);
    }
}