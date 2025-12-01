using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "AIConfiguration", menuName = "Sonar/AI/AI Configuration", order = 0)]
    public class AIConfigurationSO : ScriptableObject
    {
        [SerializeField] public bool displayStatusInUI = false;
        
        [Header("Detection and Timing")]
        public float MaxTimeWithoutEcho = 5.0f; 
    }
}