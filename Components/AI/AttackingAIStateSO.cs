using System;
using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "AttackingAIState", menuName = "Sonar/AI/States/Attacking State")]
    public class AttackingAIStateSO : BaseAIStateSO
    {
        [SerializeField] EchoEventChannelSO echoEvent;
        [SerializeField] private GameObjectAndVector3EventChannelSO primaryWeaponEvent;

        [SerializeField] private EchoTypeSO unknownEcho;
        [SerializeField] private EchoTypeSO identifiedEcho;
        [SerializeField] private EchoTypeSO disappearedEcho;

        private AIController aiController;

        [SerializeField] private BaseAIStateSO patrollingAIState;
        [SerializeField] private BaseAIStateSO chasingAIState;

        public override void Enable(AIController aiController)
        {
            this.aiController = aiController;
            echoEvent.OnEventRaised += HandleEchoEvent;
            aiController.Stop();
        }

        public override void Disable(AIController aiController)
        {
            echoEvent.OnEventRaised -= HandleEchoEvent;
        }
        public override void Behave(AIController aiController)
        {
            Debug.Log($"AI {aiController.gameObject.name}: Attacking state");
            aiController.Stop();
            Debug.Log($"Attacking: raising primary weapon event with target {aiController.ChaseTarget}");
            primaryWeaponEvent.RaiseEvent(aiController.gameObject, aiController.ChaseTarget);
        }

        private void HandleEchoEvent(Echo echo)
        {
            if (echo.DetectedObject != null)
            {
                Debug.Log($"AI {aiController.gameObject.name} Attacking: handling echo event from {echo.DetectedObject.name}");
            }

            if (aiController == null)
            {
                return;
            }

            if (aiController != null && echo.GlobalSonarContext.gameObject != aiController.gameObject)
            {
                //Debug.Log("[Patrolling State] Received an echo from another sonar context. Will not be considered.");
                return;
            }
            if (echo.EchoType == unknownEcho)
            {
                Debug.Log("Attacking: unknownEcho");
                aiController.ChaseTarget = echo.LastPosition;
                aiController.ChaseTargetEcho = echo;
                aiController.TransitionToState(chasingAIState);
                aiController.OnChasing();
            }
            else
            {
                if (echo.EchoType == disappearedEcho)
                {
                    Debug.Log($"AI {aiController.gameObject.name} Attacking: disappearedEcho");
                    aiController.TransitionToState(patrollingAIState);
                    aiController.OnPatrolling();
                }
                else
                {
                    if (echo.EchoType == identifiedEcho)
                    {
                        if (echo.DetectedObject != null && !echo.DetectedObject.TryGetComponent(out PlayerController playerController))
                        {
                            Debug.Log($"AI {aiController.gameObject.name} AI: identified echo is not a player. Continuing chase.");
                            return;
                        }
                        Debug.Log($"AI {aiController.gameObject.name} Attacking: ChaseTargetEcho == echo: {aiController.ChaseTargetEcho.DetectedObject == echo.DetectedObject}");
                        if (aiController.ChaseTargetEcho != null && aiController.ChaseTargetEcho.DetectedObject == echo.DetectedObject)
                        {
                            Debug.Log($"AI {aiController.gameObject.name} Attacking: new target position: {echo.LastPosition}");
                            aiController.ChaseTarget = echo.LastPosition;
                        }
                    }   
                }
            }
        }
    }    
}
