using System.Collections.Generic;
using Sonar;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class AIController : MonoBehaviour
{
    [SerializeField] private AIConfigurationSO aiConfigurationSO;
    [SerializeField] private NavMeshAgent navMeshAgent;
    public NavMeshAgent NavMeshAgent => navMeshAgent;

    //[SerializeField] private GameObjectAndVector3EventChannelSO MoveToDestinationEvent;
    [SerializeField] private GameObjectAndVector3EventChannelSO SetDestinationEvent;
    [SerializeField] private GameObjectEventChannelSO StopEvent;
    [SerializeField] private GameObjectAndFloatEventChannelSO MoveEvent;

    // Patrolling state
    [SerializeField] private List<Transform> waypoints;
    public List<Transform> Waypoints => waypoints;
    [SerializeField] public int CurrentWaypointIndex = 0;

    // Chasing state
    [SerializeField] public Vector3 ChaseTarget = Vector3.zero;
    [SerializeField] public Echo ChaseTargetEcho;

    // Timer available for various uses by states
    [SerializeField] public float StateTimer = 0f;

    // State management
    [SerializeField] private BaseAIStateSO currentState;
    [SerializeField] private BaseAIStateSO startingState;


    [SerializeField] private EchoTypeSO unkownEchoType;
    [SerializeField] private EchoTypeSO identifiedEchoType;
    [SerializeField] private EchoEventChannelSO echoEvent;
    [SerializeField] private GameObjectEventChannelSO useActiveSonarEvent;

    // Sonar policy
    [SerializeField] private SonarPolicySO currentSonarPolicy;

    [SerializeField] TextMeshProUGUI FeedbackDisplay;

        private void OnEnable()
        {
            if (echoEvent != null)
            {
                echoEvent.OnEventRaised += HandleEchoEvent;
            }
        }

        private void OnDisable()
        {
            if (echoEvent != null)
            {
                echoEvent.OnEventRaised += HandleEchoEvent;
            }
        }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        TransitionToState(startingState);
        OnPatrolling();

    }

    private void Update()
    {
        currentState?.Behave(this);
    }

    public void TransitionToState(BaseAIStateSO nextState)
    {
        currentState?.Disable(this);
        currentState = nextState;
        currentState?.Enable(this);
    }

    public void AddWaypoint(Transform waypoint)
    {
        waypoints.Add(waypoint);
    }

    public void GoToNextWaypoint()
    {
        if (waypoints.Count < (CurrentWaypointIndex + 2))
        {
            Debug.Log("Next waypoint index is outside the waypoints list boundaries");
            return;
        }
        int nextWaypointIndex = (CurrentWaypointIndex + 1) % Waypoints.Count;
        //Debug.Log($"Going to next waypoint: {nextWaypointIndex}");
        
    }



    public void OnPatrolling()
    {
        if (aiConfigurationSO.displayStatusInUI)
        {
            FeedbackDisplay.text = "Patrolling";   
        }
    }

    public void OnChasing()
    {
        if (aiConfigurationSO.displayStatusInUI)
        {
            FeedbackDisplay.text = "Chasing";
        }
    }

    public void OnAttacking()
    {
        if (aiConfigurationSO.displayStatusInUI)
        {
            FeedbackDisplay.text = "Attacking";
        }
    }

    private void HandleEchoEvent(Echo echo)
    {
        if (currentSonarPolicy == null)
        {
            Debug.Log("No sonar policy.");
            return;
        }
        if (echo.EchoType == unkownEchoType)
        {
            if (currentSonarPolicy.ScanOnUnknownEcho)
            {
                useActiveSonarEvent.RaiseEvent(gameObject);
            }
        }
        else
        {
            if (echo.EchoType == identifiedEchoType)
            {
                if (currentSonarPolicy.ScanOnIdentifiedEcho)
                {
                    useActiveSonarEvent.RaiseEvent(gameObject);
                }
            }
        }
    }

    public void SetDestination(Vector3 destination)
    {
        SetDestinationEvent?.RaiseEvent(gameObject, destination);
    }

    public void Move(float speed)
    {
        MoveEvent?.RaiseEvent(gameObject, speed);
    }

    public void Stop()
    {
        StopEvent?.RaiseEvent(gameObject);
    }

    public bool HasReachedDestination()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Transform GetCurrentWaypoint()
    {
        if(waypoints.Count == 0)
        {
            return null;
        }
        return waypoints[CurrentWaypointIndex];
    }
    
    public void NextPoint()
    {
        if(waypoints.Count > 0)
        {
            CurrentWaypointIndex = (CurrentWaypointIndex + 1) % waypoints.Count;
            SetDestinationEvent?.RaiseEvent(gameObject, waypoints[CurrentWaypointIndex].position);
        }
    }
}