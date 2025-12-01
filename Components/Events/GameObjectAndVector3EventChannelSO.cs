using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameObjectAndVector3", menuName = "Sonar/Events/GameObjectAndVector3 Game Event")]
public class GameObjectAndVector3EventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform when the event is raised.")]
    public UnityAction<GameObject, Vector3> OnEventRaised;
    public void RaiseEvent(GameObject value, Vector3 value2)
    {
        OnEventRaised?.Invoke(value, value2);
    }
}