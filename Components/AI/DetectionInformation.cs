using Sonar;
using UnityEngine;

public class DetectionInformation
{
    private GameObject detectedObject;
    public GameObject DetectedObject => detectedObject;
    private Vector3 detectionLocation;
    public Vector3 DetectionLocation => detectionLocation;
    private float detectionIntensity;
    public float DetectionIntensity => detectionIntensity;
    private BaseEquipment sourceSonar;
    public BaseEquipment SourceSonar => sourceSonar;

    private EquipmentController sourceEquipmentController;
    public EquipmentController SourceEquipmentController => sourceEquipmentController;

    public DetectionInformation(GameObject detectedObject, Vector3 detectionLocation, float detectionIntensity, BaseEquipment sourceSonar, EquipmentController sourceEquipmentController)
    {
        this.detectedObject = detectedObject;
        this.detectionLocation = detectionLocation;
        this.detectionIntensity = detectionIntensity;
        this.sourceSonar = sourceSonar;
        this.sourceEquipmentController = sourceEquipmentController;
    }

    public string toString()
    {
        return "DetectionInformation: " + detectedObject.name + ", " + detectionLocation + ", " + detectionIntensity + ", " + sourceSonar.name + ", " + sourceEquipmentController.name;
    }

}