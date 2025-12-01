using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "HealthCharacteristicsSO", menuName = "Sonar/Health/Health Characteristics", order = 0)]
    public class HealthCharacteristicsSO : ScriptableObject {
        [SerializeField] public float MaxHealth;
        [SerializeField] public bool Invulnerable;
    }
    
}

