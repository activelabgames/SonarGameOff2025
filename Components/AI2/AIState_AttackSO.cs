// AIState_AttackSO.cs

using UnityEngine;

namespace Sonar.AI
{
    [CreateAssetMenu(fileName = "AIState_Attack", menuName = "Sonar/AI2/States/Attack")]
    public class AIState_AttackSO : AIStateSO
    {
        [Header("Transitions")]
        [SerializeField] private AIStateSO keepDistanceState; // L'état vers lequel l'IA se retire
        [SerializeField] private AIStateSO patrolState; 
        
        [Header("Parameters")]
        [Tooltip("Distance de repli par défaut si les données de torpille sont manquantes.")]
        [SerializeField] private float fallbackFiringDistance = 50f;
        
        [Tooltip("Vitesse de mouvement pendant l'attaque (souvent plus lente ou nulle).")]
        [SerializeField] private float attackSpeed = 0f;

        [Header("Hysteresis")]
        [Tooltip("Marge de distance à respecter (en mètres) en deçà de la distance d'activation pour basculer en KeepDistance (retraite).")]
        [SerializeField] private float retreatTransitionMargin = 5.0f; // 🚀 Ajout de l'hystérésis pour la retraite
        
        // --- Logique d'accès à la distance cible ---

        private float GetDesiredFiringDistance(AIEnemyContext context)
        {
            // Tente d'utiliser la distance d'activation de la torpille.
            BaseTorpedoDataSO torpedoData = context.PrimaryTorpedoData; 
            
            if (torpedoData != null)
            {
                return torpedoData.AutomaticActivationDistance; 
            }
            
            // Si les données sont manquantes, utilise la valeur de repli
            return fallbackFiringDistance;
        }
        
        // ---------------------------------------------

        public override void OnEnter(IAIContext genericContext)
        {
            AIEnemyContext context = genericContext as AIEnemyContext;
            if (context == null) return;

            context.OnAttacking();
            context.Stop(); // L'IA s'arrête pour tirer (ajuster si nécessaire)
        }

        public override void OnUpdate(IAIContext genericContext)
        {
            AIEnemyContext context = genericContext as AIEnemyContext;
            if (context == null) return;
            
            float desiredFiringDistance = GetDesiredFiringDistance(context); 
            float distanceToTarget = Vector3.Distance(context.transform.position, context.ChaseTarget);

            // 🚀 DÉBOGAGE : Mise à jour des distances
            context.UpdateDebugDistances(distanceToTarget, desiredFiringDistance);
            
            // 1. GESTION DU TIMEOUT
            if (Time.time - context.LastDetectionTime > context.AIConfigurationSO.MaxTimeWithoutEcho)
            {
                context.Stop(); 
                context.RequestStateChange(patrolState);
                return;
            }

            // 2. CONDITION DE RETRAITE (Distance trop courte)
            // 🚨 CORRECTION CRUCIALE : On se retire SEULEMENT si la distance est plus petite que 
            // la distance idéale MOINS la marge de retraite (hystérésis).
            if (distanceToTarget < desiredFiringDistance - retreatTransitionMargin) 
            {
                Debug.Log($"AI2 Attack: Target too close ({distanceToTarget:F1}m). Retreating to KeepDistance state.");
                context.RequestStateChange(keepDistanceState);
                return;
            }
            
            // 3. LOGIQUE D'ATTAQUE (Si la distance est bonne)
            // L'IA est dans la zone de tir idéale (entre D_ideal - marge de retraite et D_ideal + marge d'attaque)
            
            // Si l'IA utilise un tir périodique (à implémenter)
            context.Attack(); 
        }

        public override void OnExit(IAIContext genericContext)
        {
            // Rien de spécifique à faire si l'IA passe à KeepDistance ou Patrol
        }
    }
}