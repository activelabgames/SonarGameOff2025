using UnityEngine;

public abstract class EmptyEventListener : MonoBehaviour
{
    [SerializeField] private EmptyEventChannelSO emptyEventChannel;

    private void OnEnable()
    {
        if (emptyEventChannel != null)
        {
            emptyEventChannel.OnEventRaised += HandleEvent;
        }
    }

    private void OnDisable()
    {
        if (emptyEventChannel != null)
        {
            emptyEventChannel.OnEventRaised -= HandleEvent;
        }
    }

    protected virtual void HandleEvent()
    {
        // Override in derived classes to handle the event.
    }
}