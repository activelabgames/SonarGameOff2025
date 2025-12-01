using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "GameStateEvent", menuName = "Sonar/Events/Game State", order = 0)]
    public class GameStateEventChannelSO : ScriptableObject
    {
        public UnityEngine.Events.UnityAction<GameStateSO> OnEventRaised;

        public void RaiseEvent(GameStateSO gameState)
        {
            OnEventRaised?.Invoke(gameState);
        }
    }
}
