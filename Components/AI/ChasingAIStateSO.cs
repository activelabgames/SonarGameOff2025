using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "PatrollingAIState", menuName = "Sonar/AI/States/Chasing State")]
    public class ChasingAIStateSO : BaseAIStateSO
    {
        [SerializeField] private float chaseSpeed;

        [SerializeField] EchoEventChannelSO echoEvent;

        //[SerializeField] private EchoTypeSO unknownEcho;
        [SerializeField] private EchoTypeSO identifiedEcho;
        [SerializeField] private EchoTypeSO disappearedEcho;

        private AIController aiController;

        [SerializeField] private BaseAIStateSO patrollingAIState;
        [SerializeField] private BaseAIStateSO attackingAIState;

        public override void Enable(AIController aiController)
        {
            this.aiController = aiController;
            echoEvent.OnEventRaised += HandleEchoEvent;
            //aiController.StateTimer = 0f;
            aiController.SetDestination(aiController.ChaseTarget);
            aiController.Move(chaseSpeed);
        }

        public override void Disable(AIController aiController)
        {
            echoEvent.OnEventRaised -= HandleEchoEvent;
        }
        public override void Behave(AIController aiController)
        {
            Debug.Log($"AI {aiController.gameObject.name}: Chasing state");
            aiController?.SetDestination(aiController.ChaseTarget);
            aiController?.Move(chaseSpeed);
        }

        private void HandleEchoEvent(Echo echo)
        {
            if (echo.GlobalSonarContext == null)
            {
                Debug.LogWarning($"AI {aiController.gameObject.name}: Echo received with no sonar context. Ignoring.");
                return;
            }
            if (aiController == null)
            {
                Debug.LogWarning($"AI {aiController.gameObject.name}: AIController is null. Cannot process echo.");
                return;
            }
            if (echo.GlobalSonarContext.gameObject != aiController.gameObject)
            {
                //Debug.Log("[Patrolling State] Received an echo from another sonar context. Will not be considered.");
                return;
            }
            if (echo.EchoType == identifiedEcho)
            {
                if (echo.DetectedObject != null && !echo.DetectedObject.TryGetComponent(out PlayerController playerController))
                {
                    Debug.Log($"AI {aiController.gameObject.name}: identified echo is not a player. Continuing chase.");
                    return;
                }
                aiController.ChaseTarget = echo.LastPosition;
                aiController.ChaseTargetEcho = echo;
                Debug.Log($"AI {aiController.gameObject.name}: from chasing state to attacking state with target position = {echo.LastPosition}");
                aiController.TransitionToState(attackingAIState);
                aiController.OnAttacking();
            }
            else
            {
                if (echo.EchoType == disappearedEcho)
                {
                    aiController.TransitionToState(patrollingAIState);
                    aiController.OnPatrolling();
                }                
            }
        }
    }    
}
