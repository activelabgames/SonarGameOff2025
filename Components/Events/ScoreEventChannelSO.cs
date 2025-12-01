using Sonar;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ScoreGameEvent", menuName = "Sonar/Events/Score Game Event")]
public class ScoreEventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform when the event is raised.")]
    public UnityAction<PlayerDataSO> OnEventRaised;
    public void RaiseEvent(PlayerDataSO value)
    {
        OnEventRaised?.Invoke(value);
    }
}