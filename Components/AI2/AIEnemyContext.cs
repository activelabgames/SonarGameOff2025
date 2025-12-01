// AIEnemyContext.cs

using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections.Generic;

namespace Sonar.AI
{
    public class AIEnemyContext : MonoBehaviour, IAIContext, IWeaponLimiter
    {
        [Header("--- References ---")]
        [SerializeField] private WeaponsController weaponsController;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] TextMeshProUGUI FeedbackDisplay; // Debug UI

        [Header("--- Configuration & Events ---")]
        [SerializeField] private AIConfigurationSO aiConfigurationSO;
        public AIConfigurationSO AIConfigurationSO => aiConfigurationSO;
        [SerializeField] private AIStateEventChannelSO stateRequestChannel;
        [SerializeField] private EchoEventChannelSO echoEvent;
        [SerializeField] private GameObjectAndVector3EventChannelSO SetDestinationEvent;
        [SerializeField] private GameObjectEventChannelSO StopEvent;
        [SerializeField] private GameObjectAndFloatEventChannelSO MoveEvent;
        [SerializeField] private GameObjectAndVector3EventChannelSO primaryWeaponEvent;

        // =================================================================
        // 🚀 AI DEBUG & MONITORING (CHAMPS SÉRIALISÉS)
        // =================================================================
        
        [Header("🤖 AI DEBUG")]
        
        [Tooltip("État de l'IA actuellement actif.")]
        [SerializeField] 
        private string _currentAIStateName = "Uninitialized"; 
        
        [Tooltip("Distance actuelle entre le sous-marin et sa cible (le joueur).")]
        [SerializeField] 
        private float _currentDistanceToTarget = 0f;

        [Tooltip("Distance de tir idéale (AutomaticActivationDistance de la torpille).")]
        [SerializeField] 
        private float _desiredFiringDistance = 0f;

        [Tooltip("Destination actuelle de la retraite sur le NavMesh.")]
        [SerializeField] 
        private Vector3 _currentRetreatDestination = Vector3.zero;

        [Tooltip("Vitesse de l'agent de navigation actuellement appliquée.")]
        [SerializeField] 
        private float _currentNavMeshSpeed = 0f;
        
        [Tooltip("Compteur de torpilles actives (pour IWeaponLimiter).")]
        [SerializeField]
        private int _activeTorpedoCount = 0; 
        
        // =================================================================
        // ⚔️ WEAPON & TARGET DATA
        // =================================================================

        // IWeaponLimiter Implementation
        public int CurrentActiveTorpedos 
        {
            get => _activeTorpedoCount;
            set => _activeTorpedoCount = value; // Assure que le champ sérialisé est mis à jour
        } 
        
        // Données d'arme pour KeepDistance
        public BaseTorpedoDataSO PrimaryTorpedoData 
        {
            get
            {
                // Assurez-vous que le champ 'primaryWeapon' dans WeaponsController est accessible
                if (weaponsController != null && weaponsController.primaryWeapon is BaseTorpedoDataSO torpedoData)
                {
                    return torpedoData;
                }
                return null;
            }
        }
        
        // IAIContext & Chase State fields
        public Echo LastEcho;
        public bool IsNewEchoPending = false;
        public float LastDetectionTime = 0f; 
        public Vector3 ChaseTarget = Vector3.zero;
        public Echo ChaseTargetEcho;

        // Attack State
        public GameObject AttackTarget;
        
        // Transition différée
        public float TimeConditionMet = 0f;
        public AIStateSO PendingTransitionState = null;
        
        // Patrol State
        [SerializeField] private List<Transform> waypoints;
        public List<Transform> Waypoints => waypoints;
        [SerializeField] public int CurrentWaypointIndex = 0;
        public float PatrolStateTimer = 0f;


        // =================================================================
        // LIFECYCLE & INITIALIZATION
        // =================================================================

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
                echoEvent.OnEventRaised -= HandleEchoEvent;
            }
        }

        private void Awake()
        {
            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }
            if (weaponsController == null)
            {
                weaponsController = GetComponent<WeaponsController>();
            }
            LastDetectionTime = Time.time; 
        }

        private void Update()
        {
            // Mise à jour continue des valeurs de débogage
            if (ChaseTarget != Vector3.zero) 
            { 
                 _currentDistanceToTarget = Vector3.Distance(transform.position, ChaseTarget); 
            }

            if (navMeshAgent != null && navMeshAgent.enabled)
            {
                _currentNavMeshSpeed = navMeshAgent.speed;
                if (navMeshAgent.hasPath)
                {
                    _currentRetreatDestination = navMeshAgent.destination;
                }
            }
        }

        // =================================================================
        // EVENT HANDLERS & AI CORE
        // =================================================================

        private void HandleEchoEvent(Echo echo)
        {
            if (echo == null) return;
            if (echo.GlobalSonarContext.gameObject != gameObject) return;

            if (echo.DetectedObject != null && echo.DetectedObject.TryGetComponent(out PlayerController playerController))
            {
                LastEcho = echo;
                IsNewEchoPending = true;
                LastDetectionTime = Time.time; 
            }
        }
        
        public Echo ConsumeEcho()
        {
            IsNewEchoPending = false;
            return LastEcho;
        }

        public void RequestStateChange(AIStateSO newState)
        {
            if (stateRequestChannel == null)
            {
                 Debug.Log($"AIEnemyContext: State request channel not assigned for {gameObject.name}");
            }
            // 🚀 DÉBOGAGE : Mise à jour du nom de l'état
            UpdateDebugState(newState); 

            Debug.Log("AIEnemyContext: received a state change request");
            stateRequestChannel.RaiseEvent(this, newState);
        }

        // =================================================================
        // IAIContext IMPLEMENTATION
        // =================================================================
        
        public void SetDestination(Vector3 destination)
        {
            SetDestinationEvent?.RaiseEvent(gameObject, destination);
            // La mise à jour du debug de destination se fait dans Update pour refléter le navMeshAgent.destination
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
            if (navMeshAgent == null || !navMeshAgent.enabled || !navMeshAgent.isOnNavMesh) return true;
            
            if (!navMeshAgent.pathPending)
            {
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    // Vérifie aussi que l'Agent s'est bien arrêté après son chemin
                    if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void OnPatrolling()
        {
            if (aiConfigurationSO.displayStatusInUI && FeedbackDisplay != null)
            {
                FeedbackDisplay.text = "Patrolling";  
            }
        }

        public Transform GetCurrentWaypoint()
        {
            if (waypoints.Count == 0) return null;
            return waypoints[CurrentWaypointIndex];
        }

        public void AddWaypoint(Transform waypoint)
        {
            waypoints.Add(waypoint);
        }
        
        public void OnChasing()
        {
            if (aiConfigurationSO != null && aiConfigurationSO.displayStatusInUI && FeedbackDisplay != null)
            {
                FeedbackDisplay.text = "Chasing";
            }
        }
        
        public void Attack()
        {
            primaryWeaponEvent?.RaiseEvent(this.gameObject, ChaseTarget); 
        }

        public void OnAttacking()
        {
            if (aiConfigurationSO.displayStatusInUI && FeedbackDisplay != null)
            {
                FeedbackDisplay.text = "Attacking";
            }
        }

        // =================================================================
        // 🛠️ MÉTHODES DE MISE À JOUR DU DÉBOGAGE
        // =================================================================
        
        public void UpdateDebugState(AIStateSO newState)
        {
            _currentAIStateName = newState.name;
        }

        // NOTE: La destination est mise à jour dans Update en lisant navMeshAgent.destination
        // public void UpdateDebugDestination(Vector3 destination) { ... } 

        // NOTE: La vitesse est mise à jour dans Update en lisant navMeshAgent.speed
        // public void UpdateDebugSpeed(float speed) { ... }
        
        public void UpdateDebugDistances(float distanceToTarget, float desiredFiringDistance)
        {
            // Cette méthode est prévue pour être appelée par KeepDistanceSO et AttackSO
            _currentDistanceToTarget = distanceToTarget;
            _desiredFiringDistance = desiredFiringDistance;
        }
    } 
}