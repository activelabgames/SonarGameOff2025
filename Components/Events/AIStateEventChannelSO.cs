using UnityEngine;

namespace Sonar.AI
{
    [CreateAssetMenu(fileName = "AIStateEvent", menuName = "Sonar/Events/AI State", order = 0)]
    public class AIStateEventChannelSO : ScriptableObject
    {
        public UnityEngine.Events.UnityAction<AIEnemyContext, AIStateSO> OnEventRaised;

        public void RaiseEvent(AIEnemyContext requester, AIStateSO newState)
        {
            OnEventRaised?.Invoke(requester, newState);
        }
    }
}
