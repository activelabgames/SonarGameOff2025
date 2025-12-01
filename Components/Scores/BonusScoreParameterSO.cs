using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "BonusParameterSO", menuName = "Sonar/Scores/Bonus Score Parameter")]
    public class BonusScoreParameterSO : BaseScoreParameterSO
    {
        public override void HandleScoreEvent(PlayerDataSO playerDataSO)
        {
            if (playerDataSO == null)
            {
                Debug.LogError("BonusScoreParameterSO: PlayerDataSO is null!");
                return;
            }
            if (playerDataSO.PlayerScore == null)
            {
                Debug.LogError("BonusScoreParameterSO: PlayerScoreDataSO is null for player " + playerDataSO.PlayerName.Value);
                return;
            }
            if (playerDataSO.PlayerScore.Points == null)
            {
                Debug.LogError("BonusScoreParameterSO: Points IntVariableSO is null for player " + playerDataSO.PlayerName.Value);
                return;
            }
            Debug.Log($"BonusScoreParameterSO: Adding {ScoreAdjustement} points to player {playerDataSO.PlayerName.Value}");
            playerDataSO.PlayerScore.CurrentRoundPoints.Value += ScoreAdjustement;
        }
    }
}
