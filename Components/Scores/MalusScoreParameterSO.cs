using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "MalusParameterSO", menuName = "Sonar/Scores/Malus Score Parameter")]
    public class MalusScoreParameterSO : BaseScoreParameterSO
    {
        [SerializeField] public int InitialPointsToGive;
        public override void HandleScoreEvent(PlayerDataSO playerDataSO)
        {
            if (playerDataSO == null)
            {
                Debug.LogError("MalusScoreParameterSO: PlayerDataSO is null!");
                return;
            }
            if (playerDataSO.PlayerScore == null)
            {
                Debug.LogError("MalusScoreParameterSO: PlayerScoreDataSO is null for player " + playerDataSO.PlayerName.Value);
                return;
            }
            if (playerDataSO.PlayerScore.Points == null)
            {
                Debug.LogError("MalusScoreParameterSO: Points IntVariableSO is null for player " + playerDataSO.PlayerName.Value);
                return;
            }
            Debug.Log($"MalusScoreParameterSO: Subtracting {ScoreAdjustement} points from player {playerDataSO.PlayerName.Value}");
            playerDataSO.PlayerScore.CurrentRoundTimeBonusPoints.Value -= ScoreAdjustement;
        }
    }
}
