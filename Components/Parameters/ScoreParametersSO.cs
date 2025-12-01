using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "GameScoreDataSO", menuName = "Sonar/Scores/Score Parameters", order = 0)]
    public class ScoreParametersSO : BaseParametersSO
    {
        [SerializeField] public BaseScoreParameterSO[] scoreParameters;
    }    
}
