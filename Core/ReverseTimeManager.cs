using UnityEngine;

namespace Sonar
{
    public class ReverseTimeManager : MonoBehaviour
    {
        [SerializeReference] RoundConfigurationSO roundConfiguration;
        [SerializeReference] private float roundElapsedTime;

        [SerializeReference] private ScoreEventChannelSO timeScoreEvent;
        [SerializeReference] private IntEventChannelSO timeEvent;
        [SerializeReference] private EmptyEventChannelSO timeoutEvent;

        private bool alreadyPerformed = false;
        private bool firstPassage = true;

        private void Start()
        {
            roundElapsedTime = roundConfiguration.RoundDuration.Value;
        }

        private void Update()
        {
            if (roundElapsedTime <= 0f && !roundConfiguration.UnlimitedTime)
            {
                timeoutEvent.RaiseEvent();
                return;
            }

            //Debug.Log(roundElapsedTime % (roundConfiguration.RoundDuration / 4));
            int seconds = Mathf.FloorToInt(roundElapsedTime % 60);

            timeEvent?.RaiseEvent(seconds);

            if (seconds % (roundConfiguration.RoundDuration.Value / 4) == 0)
            {
                Debug.Log($"firstPassage:{firstPassage}");
                if (!firstPassage)
                {
                    Debug.Log($"alreadyPerformed:{alreadyPerformed}");
                    if (!alreadyPerformed)
                    {
                        Debug.Log("TimeManager: round time / 4");
                        foreach (PlayerDataSO playerData in roundConfiguration.PlayersData)
                        {
                            timeScoreEvent?.RaiseEvent(playerData);
                        }
                        alreadyPerformed = true;
                    }
                }
                firstPassage = false;
            }
            else
            {
                alreadyPerformed = false;
            }
            roundElapsedTime -= Time.deltaTime;
            if (roundElapsedTime <= 0f)
            {
                roundElapsedTime = 0f;
            }
        }
    }    
}
