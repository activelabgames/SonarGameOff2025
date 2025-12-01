using TMPro;
using UnityEngine;
using System.Collections;

namespace Sonar
{
    public class GameHUDManager : MonoBehaviour
    {
        [SerializeField] TMP_Text timerHUDControl;
        [SerializeField] private IntEventChannelSO timeEvent;

        private void OnEnable()
        {
            timeEvent.OnEventRaised += HandleTimeEvent;
        }

        private void OnDisable()
        {
            timeEvent.OnEventRaised -= HandleTimeEvent;
        }

        private void Start()
        {

        }

        private void HandleTimeEvent(int seconds)
        {
            timerHUDControl.text = "" + seconds;
        }
    }
}
