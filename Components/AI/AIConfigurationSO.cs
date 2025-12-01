using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "AIConfiguration", menuName = "Sonar/AI/AI Configuration", order = 0)]
    public class AIConfigurationSO : ScriptableObject
    {
        [SerializeField] public bool displayStatusInUI = false;
        
        // 🚀 AJOUT DU CHAMP MANQUANT
        [Header("Detection and Timing")]
        [Tooltip("Temps maximal sans écho avant que l'IA ne passe en Patrouille ou change d'état (utilisé par plusieurs états).")]
        public float MaxTimeWithoutEcho = 5.0f; 
    }
}