using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Sonar/Data/Player", order = 0)]
    public class PlayerDataSO : BaseDataSO
    {
        [SerializeField] public StringVariableSO PlayerName;
        [SerializeField] public PlayerScoreDataSO PlayerScore;
    }    
}

