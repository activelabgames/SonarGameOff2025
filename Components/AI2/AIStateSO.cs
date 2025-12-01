using UnityEngine;
namespace Sonar.AI
{
    public abstract class AIStateSO : ScriptableObject
    {
        public abstract void OnEnter(IAIContext context);
        public abstract void OnUpdate(IAIContext context);
        public abstract void OnExit(IAIContext context);
    }    
}
