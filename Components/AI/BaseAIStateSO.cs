using UnityEngine;

namespace Sonar
{
    public abstract class BaseAIStateSO : ScriptableObject
    {
        public string StateName;
        public string StateDescription;

        public abstract void Enable(AIController aiController);

        public abstract void Disable(AIController aiController);

        public virtual void Behave(AIController aiController)
        {
            //Debug.Log($"AI State '{StateName}' is active.");
        }
    }    
}
