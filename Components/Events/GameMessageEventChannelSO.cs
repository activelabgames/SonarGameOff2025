using Sonar;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameMessageGameEvent", menuName = "Sonar/Events/GameMessage Game Event")]
public class GameMessageEventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform when the event is raised.")]
    public UnityAction<GameMessage> OnEventRaised;
    public void RaiseEvent(GameMessage value)
    {
        OnEventRaised?.Invoke(value);
    }
}