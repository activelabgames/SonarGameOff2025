using UnityEngine;

namespace Sonar.AI
{
    [CreateAssetMenu(fileName = "AIState_Patrol", menuName = "Sonar/AI2/States/Patrol")]
    public class AIState_PatrolSO : AIStateSO
    {

        // Other states
        [SerializeField] private AIStateSO chaseState;
        [SerializeField] private AIStateSO attackState;

        // Echoes types
        [SerializeField] private EchoTypeSO unknownEcho;
        [SerializeField] private EchoTypeSO identifiedEcho;
        [SerializeField] private EchoTypeSO activelyIdentifiedEcho;

        // Parameters
        [SerializeField] private float waitTime = 3.0f;
        [SerializeField] private float patrolSpeed = 3.0f;

        public override void OnUpdate(IAIContext genericContext)
        {
            Debug.Log($"AI2: Patrol stat - OnUpdate");

            if (genericContext == null)
            {
                Debug.Log("AI2 patrol: context is null.");
                return;
            }

            AIEnemyContext context = genericContext as AIEnemyContext;

            if (context == null)
            {
                Debug.Log("AI2 patrol: bad context was provided.");
                return;
            }

            Debug.Log($"AI2: Has reached destination = {context.HasReachedDestination()}");
            if (context.HasReachedDestination())
            {
                context.Stop();

                context.PatrolStateTimer += Time.deltaTime;

                if (context.PatrolStateTimer >= waitTime)
                {
                    if (context.Waypoints.Count > 0)
                    {
                        context.CurrentWaypointIndex = (context.CurrentWaypointIndex + 1) % context.Waypoints.Count;

                        context.SetDestination(context.GetCurrentWaypoint().position);
                        context.Move(patrolSpeed);                        
                    }
                    context.PatrolStateTimer = 0f;
                }
            }

            HandleReceivedEchoes(context);
        }

        private void HandleReceivedEchoes(AIEnemyContext context)
        {
            if(context == null)
            {
                Debug.LogFormat("AI2 patrol: context is null");
                return;
            }
            if (context.IsNewEchoPending)
            {
                Echo echo = context.ConsumeEcho();
                Debug.Log($"AI2 patrol: handling echo event about echo {echo.toString()}.");
                if (echo.GlobalSonarContext.gameObject != context.gameObject)
                {
                    Debug.Log("AI2 patrol: received an echo from another sonar context. Will not be considered.");
                    return;
                }
                if (echo.EchoType == unknownEcho)
                {
                    if (echo.DetectedObject == null)
                    {
                        Debug.LogFormat("AI2 patrol: detected object is null");
                        return;
                    }
                    context.ChaseTarget = echo.LastPosition;
                    context.ChaseTargetEcho = echo;
                    context.RequestStateChange(chaseState);
                    context.OnChasing();
                }
                else
                {
                    if (echo.EchoType == identifiedEcho || echo.EchoType == activelyIdentifiedEcho)
                    {
                        if (echo.DetectedObject != null && !echo.DetectedObject.TryGetComponent(out PlayerController playerController))
                        {
                            Debug.Log($"AI2 patrol: identified echo is not a player. Continuing chase.");
                            return;
                        }
                        context.ChaseTarget = echo.LastPosition;
                        context.ChaseTargetEcho = echo;
                        Debug.Log($"AI2 patrol: from patrolling state to attacking state with target position = {echo.LastPosition}");
                        context.RequestStateChange(attackState);
                        context.OnAttacking();
                    }
                }
            }
        }

        public override void OnEnter(IAIContext genericContext)
        {
            Debug.Log($"AI2: entering patrol state");

            if (genericContext == null)
            {
                Debug.Log("AI2 patrol: context is null.");
                return;
            }

            AIEnemyContext context = genericContext as AIEnemyContext;

            if (context == null)
            {
                Debug.Log("AI2 patrol: bad context was provided.");
                return;
            }

            context.PatrolStateTimer = 0f;
            if (context.Waypoints.Count > 0)
            {
                context.SetDestination(context.GetCurrentWaypoint().position);
                context.Move(patrolSpeed);
            }
        }

        public override void OnExit(IAIContext context)
        {

        }
    }
}
