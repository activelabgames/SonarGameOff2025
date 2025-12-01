using TMPro;
using UnityEngine;
using System.Collections;

namespace Sonar
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject gameHUDCanvas;

        [SerializeField] TMP_Text timerHUDControl;
        [SerializeField] TMP_Text roundDurationHUDControl;
        [SerializeField] TMP_Text remainingEnemiesHUDControl;
        [SerializeField] GameObject gameMessageCanvas;
        [SerializeField] TMP_Text gameMessageControl;

        [SerializeField] GameObject gamePauseCanvas;

        [SerializeReference] RoundConfigurationSO roundConfiguration;        
        [SerializeReference] IntVariableSO remainingEnemies;        

        [SerializeField] BoolEventChannelSO pauseEvent;
        [SerializeField] GameMessageEventChannelSO gameMessageEvent;
        [SerializeField] EmptyEventChannelSO messageEndEvent;

        //[SerializeField] EmptyEventChannelSO hideScoreEvent;
        [SerializeReference] private IntEventChannelSO timeEvent;

        private void OnEnable()
        {
            pauseEvent.OnEventRaised += HandlePauseEvent;
            gameMessageEvent.OnEventRaised += HandleDisplayGameMessageEvent;
            //hideScoreEvent.OnEventRaised += HandleHideScoreEvent;
            timeEvent.OnEventRaised += HandleTimeEvent;
        }

        private void OnDisable()
        {
            pauseEvent.OnEventRaised -= HandlePauseEvent;
            gameMessageEvent.OnEventRaised -= HandleDisplayGameMessageEvent;
            timeEvent.OnEventRaised -= HandleTimeEvent;
        }

        private void Start()
        {

        }

        private void HandleDisplayGameMessageEvent(GameMessage gameMessage)
        {
            StartCoroutine(DisplayMessage(gameMessage));
        }

        private IEnumerator DisplayMessage(GameMessage message)
        {
            if (message.PauseGame)
            {
                pauseEvent?.RaiseEvent(true);
            }
            gameMessageCanvas.SetActive(true);
            gameMessageControl.text = message.Message;
            yield return new WaitForSecondsRealtime(message.DisplayTime);
            gameMessageCanvas.SetActive(false);
            if (message.PauseGame)
            {
                pauseEvent?.RaiseEvent(false);
            }
            messageEndEvent?.RaiseEvent();
        }

 
        private void HandlePauseEvent(bool needPauseGame)
        {
            Debug.Log($"UIManager: pause event {needPauseGame}");
            TogglePauseGame(needPauseGame);
        }

        private void TogglePauseGame(bool needPauseGame)
        {
            if (needPauseGame)
            {
                //gameHUDCanvas.SetActive(false);
                //gamePauseCanvas.SetActive(true);
            }
        }

        private void HandleTimeEvent(int seconds)
        {
            timerHUDControl.text = "Elapsed time: " + seconds;
            roundDurationHUDControl.text = "Round duration: " + roundConfiguration.RoundDuration;
            remainingEnemiesHUDControl.text = "Remaining enemies: " + remainingEnemies.Value;
        }
    }
}
