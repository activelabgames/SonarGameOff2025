using System.Collections.Generic;
using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Sonar/Data/Game")]
    public class GameDataSO : BaseDataSO
    {
        [SerializeField]
        public IntVariableSO CurrentRoundIndex;
        [SerializeField]
        public PlayerDataSO PlayerData;
        [SerializeField]
        public PlayerDataSO EnemyData;

        [SerializeField] public IntVariableSO RemainingEnemies;

        [SerializeField]
        public List<PlayerScoreDataSO> PlayersScores = new List<PlayerScoreDataSO>();

        private void OnEnable()
        {
            SetDefaultValues();
        }

        private void OnDisable()
        {
            SetDefaultValues();
        }
        public void SetDefaultValues()
        {
            Debug.Log("GameDataSO: Setting default values.");
            CurrentRoundIndex.Value = 0;
        }
    }
}

