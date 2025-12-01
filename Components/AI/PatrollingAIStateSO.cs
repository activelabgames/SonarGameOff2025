using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "PatrollingAIState", menuName = "Sonar/AI/States/Patrolling State")]
    public class PatrollingAIStateSO : BaseAIStateSO
    {
        [SerializeField] private float patrolSpeed;
        [SerializeField] private float waitTime;

        [SerializeField] EchoEventChannelSO echoEvent;

        [SerializeField] private EchoTypeSO unknownEcho;
        [SerializeField] private EchoTypeSO identifiedEcho;

        private AIController aiController;

        [SerializeField] private BaseAIStateSO chasingAIState;
        [SerializeField] private BaseAIStateSO attackingAIState;

        public override void Enable(AIController aiController)
        {
            this.aiController = aiController;
            echoEvent.OnEventRaised += HandleEchoEvent;
            aiController.StateTimer = 0f;
            if (aiController.Waypoints.Count > 0)
            {
                aiController.SetDestination(aiController.GetCurrentWaypoint().position);
                aiController.Move(patrolSpeed);
            }
        }

        public override void Disable(AIController aiController)
        {
            echoEvent.OnEventRaised -= HandleEchoEvent;
        }
        public override void Behave(AIController aiController)
        {
            Debug.Log($"AI {aiController.gameObject.name}: Patrolling state");
            if (aiController.Waypoints.Count == 0)
            {
                Debug.LogWarning($"AI {aiController.gameObject.name}: No waypoints set for PatrollingAIState.");
                return;
            }

            if (aiController.HasReachedDestination())
            {
                aiController.Stop();

                aiController.StateTimer += Time.deltaTime;

                if (aiController.StateTimer >= waitTime)
                {
                    aiController.CurrentWaypointIndex = (aiController.CurrentWaypointIndex + 1) % aiController.Waypoints.Count;

                    aiController.SetDestination(aiController.GetCurrentWaypoint().position);
                    aiController.Move(patrolSpeed);

                    aiController.StateTimer = 0f;
                }
            }
        }

        private void HandleEchoEvent(Echo echo)
        {
            Debug.Log($"AIController is null: {aiController == null}");
            if(aiController == null)
            {
                return;
            }
            Debug.Log($"AI {aiController.gameObject.name}: handling echo event about echo {echo.toString()}.");
            if (echo.GlobalSonarContext.gameObject != aiController.gameObject)
            {
                //Debug.Log("[Patrolling State] Received an echo from another sonar context. Will not be considered.");
                return;
            }
            if (echo.EchoType == unknownEcho)
            {
                if (echo.DetectedObject == null)
                {
                    return;
                }
                aiController.ChaseTarget = echo.DetectedObject.transform.position;
                aiController.ChaseTargetEcho = echo;
                aiController.TransitionToState(chasingAIState);
                aiController.OnChasing();
            }
            else
            {
                if (echo.EchoType == identifiedEcho)
                {
                    if (echo.DetectedObject != null && !echo.DetectedObject.TryGetComponent(out PlayerController playerController))
                    {
                        Debug.Log($"AI {aiController.gameObject.name}: identified echo is not a player. Continuing chase.");
                        return;
                    }
                    aiController.ChaseTarget = echo.LastPosition;
                    aiController.ChaseTargetEcho = echo;
                    Debug.Log($"AI {aiController.gameObject.name}: from patrolling state to attacking state with target position = {echo.LastPosition}");
                    aiController.TransitionToState(attackingAIState);
                    aiController.OnAttacking();
                }
            }
            
        }
    }    
}
