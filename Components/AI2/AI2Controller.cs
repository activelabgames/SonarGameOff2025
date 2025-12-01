
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Sonar.AI
{
    public class AIController: MonoBehaviour
    {
        [SerializeField] private AIEnemyContext enemyContext;

        [SerializeField] private AIStateSO initialState;

        [SerializeField] private AIStateEventChannelSO stateRequestChannel;

        [SerializeField] private AIStateSO currentState;

        private void OnEnable()
        {
            if (stateRequestChannel != null)
                stateRequestChannel.OnEventRaised += HandleStateChangeRequest;
        }

        private void OnDisable()
        {
            if (stateRequestChannel != null)
                stateRequestChannel.OnEventRaised -= HandleStateChangeRequest;
        }

        private void Start()
        {
            if (enemyContext == null)
            {
                enemyContext = GetComponent<AIEnemyContext>();
            }

            currentState = initialState;

            if (currentState != null) currentState.OnEnter(enemyContext);
        }

        private void Update()
        {
            Debug.Log("AI2Controller: Update");
            if (currentState != null) currentState.OnUpdate(enemyContext);
        }

        private void ChangeState(AIStateSO newState)
        {
            Debug.Log($"AI2Controller: Handling new changestate request from {currentState} to {newState}");
            if (newState == null || newState == currentState) return;

            currentState.OnExit(enemyContext);
            currentState = newState;
            newState.OnEnter(enemyContext);
        }

        private void HandleStateChangeRequest(AIEnemyContext requester, AIStateSO newState)
        {
            Debug.Log("AI2Controller: new statechangerequest received");
            if (requester == enemyContext)
                ChangeState(newState);
        }
    }

}