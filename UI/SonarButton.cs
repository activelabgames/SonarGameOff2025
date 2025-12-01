using UnityEngine;
using UnityEngine.UI;

public class SonarButton : MonoBehaviour
{
    [SerializeField] private EmptyEventChannelSO SonarEvent;

    private void HandleClickEvent()
    {
        SonarEvent?.RaiseEvent();
    }
}