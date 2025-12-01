using UnityEngine;

namespace Sonar
{
    public abstract class BaseScoreParameterSO : BaseParametersSO
    {
        [SerializeField] public string ParameterName;
        [SerializeField] public string Description;
        [SerializeField] public ScoreEventChannelSO ScoreEvent;
        [SerializeField] public int ScoreAdjustement;

        private void OnEnable()
        {
            if (ScoreEvent != null)
            {
                ScoreEvent.OnEventRaised += HandleScoreEvent;
            }
        }
        
        private void OnDisable()
        {
            if (ScoreEvent != null)
            {
                ScoreEvent.OnEventRaised -= HandleScoreEvent;
            }
        }

        public abstract void HandleScoreEvent(PlayerDataSO playerDataSO);
    }
}
