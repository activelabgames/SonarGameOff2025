using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameObjectGameEvent", menuName = "Sonar/Events/GameObject Game Event")]
public class GameObjectEventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform when the event is raised.")]
    public UnityAction<GameObject> OnEventRaised;
    public void RaiseEvent(GameObject value)
    {
        OnEventRaised?.Invoke(value);
    }
}