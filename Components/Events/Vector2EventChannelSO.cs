using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Vector2GameEvent", menuName = "Sonar/Events/Vector2 Game Event")]
public class Vector2EventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform when the event is raised.")]
    public UnityAction<Vector2> OnEventRaised;
    public void RaiseEvent(Vector2 value)
    {
        OnEventRaised?.Invoke(value);
    }
}