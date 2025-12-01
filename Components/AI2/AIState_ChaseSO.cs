using UnityEngine;

namespace Sonar.AI
{
    [CreateAssetMenu(fileName = "AIState_Chase", menuName = "Sonar/AI2/States/Chase")]
    public class AIState_ChaseSO : AIStateSO
    {

        // Other states
        [SerializeField] private AIStateSO attackState;
        [SerializeField] private AIStateSO patrolState;

        // Echoes types
        [SerializeField] private EchoTypeSO unknownEcho;
        [SerializeField] private EchoTypeSO identifiedEcho;
        [SerializeField] private EchoTypeSO activelyIdentifiedEcho;
        [SerializeField] private EchoTypeSO disappearedEcho;

        // Parameters
        [SerializeField] private float chaseSpeed;


        public override void OnUpdate(IAIContext genericContext)
        {
            Debug.Log($"AI2: Chase state - OnUpdate");

            if (genericContext == null)
            {
                Debug.Log("AI2 Chase: context is null.");
                return;
            }

            AIEnemyContext context = genericContext as AIEnemyContext;

            if (context == null)
            {
                Debug.Log("AI2 Chase: bad context was provided.");
                return;
            }

            Debug.Log($"AI2 Chase: Chasing state");
            context.SetDestination(context.ChaseTarget);
            context.Move(chaseSpeed);

            HandleReceivedEchoes(context);
        }

        private void HandleReceivedEchoes(AIEnemyContext context)
        {
            if(context == null)
            {
                return;
            }
            if (context.IsNewEchoPending)
            {
                Echo echo = context.ConsumeEcho();
                Debug.Log($"AI2 Chase: handling echo event about echo {echo.toString()}.");
                if (echo.DetectedObject != null)
                {
                    Debug.Log($"AI2 Chase from {context.gameObject.name}: handling echo event from {echo.DetectedObject.name}");
                }
                if (echo.EchoType == identifiedEcho || echo.EchoType == activelyIdentifiedEcho)
                {
                    if (echo.DetectedObject != null && !echo.DetectedObject.TryGetComponent(out PlayerController playerController))
                    {
                        Debug.Log($"AI2 Chase: identified echo is not a player. Patrolling again.");
                        context.RequestStateChange(patrolState);
                        context.OnPatrolling();
                        return;
                    }
                    context.ChaseTarget = echo.LastPosition;
                    context.ChaseTargetEcho = echo;
                    context.AttackTarget = echo.DetectedObject;
                    Debug.Log($"AI2 Chase: from chasing state to attacking state with target position = {echo.LastPosition}");
                    context.RequestStateChange(attackState);
                    context.OnAttacking();
                }
                else
                {
                    if (echo.EchoType == disappearedEcho)
                    {
                        context.RequestStateChange(patrolState);
                        context.OnPatrolling();
                    }                
                }
            }
        }

        public override void OnEnter(IAIContext genericContext)
        {
            Debug.Log($"AI2: Chase state");

            if (genericContext == null)
            {
                Debug.Log("AI2 Chase: context is null.");
                return;
            }

            AIEnemyContext context = genericContext as AIEnemyContext;

            if (context == null)
            {
                Debug.Log("AI2 Chase: bad context was provided.");
                return;
            }

            context.SetDestination(context.ChaseTarget);
            context.Move(chaseSpeed);
        }

        public override void OnExit(IAIContext context)
        {

        }
    }
}
