using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameObjectAndFloatGameEvent", menuName = "Sonar/Events/GameObject And Float Game Event")]
public class GameObjectAndFloatEventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform when the event is raised.")]
    public UnityAction<GameObject, float> OnEventRaised;
    public void RaiseEvent(GameObject value1, float value2)
    {
        OnEventRaised?.Invoke(value1, value2);
    }
}