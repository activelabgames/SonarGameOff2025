// AIState_KeepDistanceSO.cs

using UnityEngine;

namespace Sonar.AI
{
    [CreateAssetMenu(fileName = "AIState_KeepDistance", menuName = "Sonar/AI2/States/Keep Distance")]
    public class AIState_KeepDistanceSO : AIStateSO
    {
        [Header("Transitions")]
        [SerializeField] private AIStateSO attackState; 
        [SerializeField] private AIStateSO patrolState; 

        [Header("Parameters")]
        [Tooltip("Multiplicateur pour calculer la distance maximale où l'IA va fuir.")]
        [SerializeField] private float maxRetreatDistanceMultiplier = 2.0f; 
        
        [Tooltip("Vitesse de mouvement pendant l'état de retraite.")]
        [SerializeField] private float retreatSpeed = 5.0f; 
        
        [Tooltip("Distance de repli par défaut si les données de torpille sont manquantes.")]
        [SerializeField] private float fallbackFiringDistance = 50f; 
        
        [Header("Hysteresis")]
        [Tooltip("Marge de distance à dépasser (en mètres) au-delà de la distance d'activation pour basculer en Attack.")]
        [SerializeField] private float attackTransitionMargin = 5.0f; 
        
        private const float MinTimeInState = 1.0f; 
        private float entryTime;
        private Vector3 retreatDestination;

        // --- Logique d'accès à la distance cible ---

        private float GetDesiredFiringDistance(AIEnemyContext context)
        {
            // Tente d'utiliser la distance d'activation de la torpille.
            BaseTorpedoDataSO torpedoData = context.PrimaryTorpedoData; 
            
            if (torpedoData != null)
            {
                return torpedoData.AutomaticActivationDistance; 
            }
            
            // Debug.LogError("TorpedoDataSO (Primary) is missing on AIEnemyContext! Using fallback distance.");
            return fallbackFiringDistance;
        }

        // ---------------------------------------------
        
        public override void OnEnter(IAIContext genericContext)
        {
            AIEnemyContext context = genericContext as AIEnemyContext;
            if (context == null) return;
            
            entryTime = Time.time;
            context.OnChasing(); 
            
            // Calcul et définition de la première destination de retraite
            retreatDestination = CalculateRetreatDestination(context);
            context.SetDestination(retreatDestination);
            
            // Démarrage du mouvement
            context.Move(retreatSpeed);
        }

        public override void OnUpdate(IAIContext genericContext)
        {
            AIEnemyContext context = genericContext as AIEnemyContext;
            if (context == null) return;
            
            float desiredFiringDistance = GetDesiredFiringDistance(context); 
            float distanceToTarget = Vector3.Distance(context.transform.position, context.ChaseTarget);

            // 🚀 DÉBOGAGE : Mise à jour des distances dans le contexte
            context.UpdateDebugDistances(distanceToTarget, desiredFiringDistance);

            // DEBUG RAYCAST : Visualisation de la destination de retraite
            Debug.DrawLine(context.transform.position, retreatDestination, Color.yellow);

            // 1. GESTION DU TIMEOUT (Perte de l'écho)
            if (Time.time - context.LastDetectionTime > context.AIConfigurationSO.MaxTimeWithoutEcho)
            {
                context.Stop(); 
                context.RequestStateChange(patrolState);
                return;
            }

            // 2. VÉRIFICATION DE LA DISTANCE (Condition de sortie vers ATTACK)
            // L'IA passe en Attack seulement si elle est au-delà de la distance idéale PLUS la marge.
            if (distanceToTarget >= desiredFiringDistance + attackTransitionMargin) 
            {
                context.Stop(); 
                context.RequestStateChange(attackState);
                return;
            }

            // 3. MISE À JOUR DE LA DESTINATION (si le point précédent est atteint)
            if (context.HasReachedDestination() && Time.time > entryTime + MinTimeInState)
            {
                // On recalcule un nouveau point de retraite plus éloigné
                retreatDestination = CalculateRetreatDestination(context);
                context.SetDestination(retreatDestination);
            }
        }
        
        private Vector3 CalculateRetreatDestination(AIEnemyContext context)
        {
            float desiredFiringDistance = GetDesiredFiringDistance(context); 

            Vector3 aiPosition = context.transform.position;
            Vector3 targetPosition = context.ChaseTarget;
            
            Vector3 directionToRetreat = (aiPosition - targetPosition).normalized;
            // Calcule une destination deux fois plus éloignée que la distance de tir idéale
            float retreatDistance = desiredFiringDistance * maxRetreatDistanceMultiplier;

            Vector3 newDestination = aiPosition + directionToRetreat * retreatDistance;

            // Assurer que la destination est atteignable sur le NavMesh
            if (UnityEngine.AI.NavMesh.SamplePosition(newDestination, out UnityEngine.AI.NavMeshHit hit, retreatDistance, UnityEngine.AI.NavMesh.AllAreas))
            {
                return hit.position;
            }
            
            return newDestination;
        }

        public override void OnExit(IAIContext genericContext)
        {
            AIEnemyContext context = genericContext as AIEnemyContext;
            if (context == null) return;

            context.Stop(); 
        }
    }
}