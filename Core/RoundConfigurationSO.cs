using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "RoundConfigurationSO", menuName = "Sonar/Configuration/Round", order = 0)]
    public class RoundConfigurationSO : ScriptableObject {
        [SerializeField] public FloatVariableSO RoundDuration;
        [SerializeField] public bool UnlimitedTime;
        [SerializeField] public int EnemiesNumber = 1;
        [SerializeField] public PlayerDataSO[] PlayersData;
        [SerializeField] public float MinEnemySpawnDistanceFromPlayer = 50.0f;

        [SerializeField] public GameParametersSO GameParameters;
    }    
}

