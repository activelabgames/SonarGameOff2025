using UnityEngine;
using UnityEngine.Events;

namespace Sonar
{
    [CreateAssetMenu(fileName = "EchoGameEvent", menuName = "Sonar/Events/Echo Game Event")]
    public class EchoEventChannelSO : ScriptableObject
    {
        [Tooltip("The action to perform when the event is raised.")]
        public UnityAction<Echo> OnEventRaised;
        public void RaiseEvent(Echo echo)
        {
            OnEventRaised?.Invoke(echo);
        }
    }
}
