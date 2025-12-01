using UnityEngine;

namespace Sonar
{
    public class Echo
    {
        public GlobalSonarContext GlobalSonarContext;
        public GameObject DetectedObject;
        public EchoTypeSO EchoType;
        public float LastDetectionTimer;
        public Vector3 LastPosition;
        public int AmountOfDetection;

        public Echo(GlobalSonarContext globalSonarContext, GameObject detectedObject, EchoTypeSO echoType, float lastDetectionTimer, Vector3 lastPosition, int amountOfDetection)
        {
            GlobalSonarContext = globalSonarContext;
            DetectedObject = detectedObject;
            EchoType = echoType;
            LastDetectionTimer = lastDetectionTimer;
            LastPosition = lastPosition;
            AmountOfDetection = amountOfDetection;
        }

        public string toString()
        {
            if(GlobalSonarContext != null && DetectedObject != null)
            {
                return "Echo from " + GlobalSonarContext.name + " with data = " + DetectedObject.name + ", " + EchoType + ", " + LastDetectionTimer;
            }
            return "";
        }
    }
}
