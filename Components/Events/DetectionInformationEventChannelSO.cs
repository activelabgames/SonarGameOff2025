using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DetectionInformationGameEvent", menuName = "Sonar/Events/DetectionInformation Game Event")]
public class DetectionInformationEventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform when the event is raised.")]
    public UnityAction<DetectionInformation> OnEventRaised;
    public void RaiseEvent(DetectionInformation detectionInformation)
    {
        Debug.Log($"Source {detectionInformation.SourceSonar} from {detectionInformation.SourceEquipmentController.gameObject.name} detected {detectionInformation.DetectedObject} with intensity {detectionInformation.DetectionIntensity} at location {detectionInformation.DetectionLocation}");
        OnEventRaised?.Invoke(detectionInformation);
    }
}