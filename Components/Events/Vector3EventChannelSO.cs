using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Vector3GameEvent", menuName = "Sonar/Events/Vector3 Game Event")]
public class Vector3EventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform when the event is raised.")]
    public UnityAction<Vector3> OnEventRaised;
    public void RaiseEvent(Vector3 value)
    {
        OnEventRaised?.Invoke(value);
    }
}