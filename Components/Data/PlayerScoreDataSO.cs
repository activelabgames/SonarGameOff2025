using System;
using UnityEngine;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.UIElements;

namespace Sonar
{
    [CreateAssetMenu(fileName = "PlayerScoreDataSO", menuName = "Sonar/Data/Scores/Player Score", order = 0)]
    public class PlayerScoreDataSO : ScriptableObject, INotifyBindablePropertyChanged
    {
        [SerializeField] public PlayerDataSO Player;
        [SerializeField] private IntVariableSO m_CurrentRoundPoints;
        [SerializeField] private IntVariableSO m_CurrentRoundTimeBonusPoints;
        [SerializeField] private IntVariableSO m_Points;
        public IntVariableSO Points => m_Points;
        public IntVariableSO CurrentRoundPoints => m_CurrentRoundPoints;
        public IntVariableSO CurrentRoundTimeBonusPoints => m_CurrentRoundTimeBonusPoints;

        [SerializeField] public int CurrentRoundTotalPoints => m_CurrentRoundPoints.Value + m_CurrentRoundTimeBonusPoints.Value;

        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(propertyName));
        }

        private void OnEnable()
        {
            if (m_CurrentRoundPoints != null)
            {
                m_CurrentRoundPoints.OnValueChanged += HandlePointsChanged;
            }
            if (m_CurrentRoundTimeBonusPoints != null)
            {
                m_CurrentRoundTimeBonusPoints.OnValueChanged += HandlePointsChanged;
            }
        }

        private void OnDisable()
        {
            if (m_CurrentRoundPoints != null)
            {
                m_CurrentRoundPoints.OnValueChanged -= HandlePointsChanged;
            }
            if (m_CurrentRoundTimeBonusPoints != null)
            {
                m_CurrentRoundTimeBonusPoints.OnValueChanged -= HandlePointsChanged;
            }
        }

        private void HandlePointsChanged(int newValue)
        {
            Debug.Log($"PlayerScoreDataSO: Points changed for player {Player.PlayerName.Value}. New total points: {CurrentRoundTotalPoints}");
            OnPropertyChanged(nameof(CurrentRoundTotalPoints));
        }
    }
}
