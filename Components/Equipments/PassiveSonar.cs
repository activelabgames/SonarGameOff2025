using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sonar
{
    public class PassiveSonar : BaseEquipment
    {
        [SerializeField] private DetectionInformationEventChannelSO detectionInformationEvent;
        [SerializeField] private DetectionInformationEventChannelSO identificationInformationEvent;
        [SerializeField] private DetectionInformationEventChannelSO lostInformationEvent;
        [SerializeField] private SoundInformationEventChannelSO soundInformationEvent;

        [SerializeField] private GlobalSonarContextSO globalSonarContextSO;

        [SerializeField] private bool sonarGizmoEnabled = false;

        private PassiveSonarDataSO passiveSonarDataSO;

        private void OnEnable()
        {
            if (soundInformationEvent != null)
            {
                soundInformationEvent.OnEventRaised += HandleSoundInformationEvent;
            }
        }

        private void OnDisable()
        {
            if (soundInformationEvent != null)
            {
                soundInformationEvent.OnEventRaised += HandleSoundInformationEvent;
            }
        }

        public void Init(EquipmentController theEquipmentController, PassiveSonarDataSO passiveSonarDataSO,GlobalSonarContextSO globalSonarContextSO)
        {
            this.passiveSonarDataSO = passiveSonarDataSO;
            this.globalSonarContextSO = globalSonarContextSO;
            sonarGizmoEnabled = passiveSonarDataSO.IsGizmoEnabled;
            base.Init(theEquipmentController, passiveSonarDataSO);
        }

        private void Awake()
        {
            
        }

        private void Start()
        {

        }

        private void Update()
        {

        }

        public void HandleSoundInformationEvent(SoundInformation soundInformation)
        {
            if (!IsInitialized)
            {
                return;
            }

            if (EquipmentController == null)
            {
                return;
            }
            if (soundInformation.Source.gameObject == EquipmentController.gameObject)
            {
                return;
            }

            if(soundInformation.Source != null)
            {
                soundInformation.Source.TryGetComponent<DetectableObject>(out DetectableObject sourceEC);
                if (sourceEC == null)
                {
                    return;
                }
            }
            Debug.Log($"Passive Sonar: Intensity: {soundInformation.Intensity}");

            PassiveSonarDataSO passiveSonarDataSO = EquipmentData as PassiveSonarDataSO;

            Debug.Log($"transform.position: {transform.position}, soundInformation.Location: {soundInformation.Location}, distance = {Vector3.Distance(transform.position, soundInformation.Location)}");

            float distance = Vector3.Distance(transform.position, soundInformation.Location);

            Debug.Log($"Passive Sonar: Distance: {distance}");

            float detectionIntensity = -1f;

            if (distance <= 0.1f)
            {
                detectionIntensity = 1f;
            }
            else
            {
                if (distance >= passiveSonarDataSO.DetectionRange)
                {
                    detectionIntensity = 0f; // Hors de portée
                }
                else
                {
                    float decayFactor = 1f - (distance / passiveSonarDataSO.DetectionRange);
                    //float decayFactorSquared = decayFactor * decayFactor * decayFactor;
                    float decayFactorPowered = 0f;

                    if (decayFactor > 0f)
                    {
                        decayFactorPowered = Mathf.Pow(decayFactor, passiveSonarDataSO.DecayExponent);
                    }

                    // We divide engine sound intensity by the max engine intensity we manage to normalize big values
                    float normalizedSoundIntensity = soundInformation.Intensity / passiveSonarDataSO.MaxEngineIntensityHandled;

                    detectionIntensity = normalizedSoundIntensity * decayFactorPowered;
                    Debug.Log($"Passive Sonar: Detection intensity before clamp: {detectionIntensity}");
                    detectionIntensity = Mathf.Clamp(detectionIntensity, 0f, 1f);
                }


            }

            Debug.Log($"Passive Sonar: Detection intensity: {detectionIntensity.ToString("G30")}");

            GameObject detectedGO = soundInformation.Source;

            detectedGO.TryGetComponent<EquipmentController>(out EquipmentController detectedEC);

            if (detectedEC != null)
            {
                detectedEC.SoundIntensity = soundInformation.Intensity;
                detectedEC.DetectionIntensity = detectionIntensity;
            }

            const float MinDetectionIntensity = 0.001f;

            float identificationRatio = passiveSonarDataSO.IdentificationRatio;

            float minIdentificationIntensity = Mathf.Pow(1f - identificationRatio, passiveSonarDataSO.DecayExponent);

            Debug.Log($"Passive Sonar: Min Detection Intensity: {MinDetectionIntensity}");
            Debug.Log($"Passive Sonar: Min identification Intensity: {minIdentificationIntensity}");

            if (detectionIntensity >= MinDetectionIntensity && detectionIntensity < minIdentificationIntensity)
            {
                Debug.Log("Passive Sonar: Detection!");
                detectionInformationEvent.RaiseEvent(new DetectionInformation(
                        soundInformation.Source,
                        soundInformation.Location,
                        detectionIntensity,
                        this,
                        EquipmentController));
                /*detectionsBuffer.Add(new DetectionInformation(
                soundInformation.Source,
                soundInformation.Location,
                detectionIntensity,
                this,
                EquipmentController));*/
            }

            if (detectionIntensity > minIdentificationIntensity)
            {
                Debug.Log("Passive Sonar: Identification!");
                identificationInformationEvent.RaiseEvent(new DetectionInformation(
                        soundInformation.Source,
                        soundInformation.Location,
                        detectionIntensity,
                        this,
                        EquipmentController));
                /*identificationsBuffer.Add(new DetectionInformation(
                        soundInformation.Source,
                        soundInformation.Location,
                        detectionIntensity,
                        this,
                        EquipmentController));*/
            }

            if (detectionIntensity == 0f)
            {
                Debug.Log("Passive Sonar: Detection was lost!");
                lostInformationEvent.RaiseEvent(new DetectionInformation(
                        soundInformation.Source,
                        soundInformation.Location,
                        detectionIntensity,
                        this,
                        EquipmentController));
                /*lostsBuffer.Add(new DetectionInformation(
                        soundInformation.Source,
                        soundInformation.Location,
                        detectionIntensity,
                        this,
                        EquipmentController));*/
            }

                //Debug.Log($"Passive Sonar '{EquipmentController.gameObject.name}' detected sound from '{soundInfo.Source}' at distance {distance} and intensity {soundInfo.Intensity}. Calculated detection Intensity: {detectionIntensity}");
                
                /*if (detectionIntensity > 0.0f)
                {*/
                    //Debug.Log($"Passive Sonar '{EquipmentController.gameObject.name}' detected sound from '{soundInfo.Source}' at distance {distance}.");
                   /* detectionInformationEvent.RaiseEvent(new DetectionInformation(
                        soundInfo.Source,
                        soundInfo.Location,
                        detectionIntensity,
                        this,
                        EquipmentController));*/
                /*}*/
        }

        private void OnDrawGizmos()
        {
            // Vérifie si le ScriptableObject est assigné
            if (EquipmentData == null)
            {
                Debug.Log("Equipment is null.");
                return;
            }

            // 2. Dessiner la sphère pleine (ou DrawWireSphere pour un contour)
            // La position est celle du GameObject. Le rayon est dans le ScriptableObject.
            PassiveSonarDataSO passiveSonarDataSO = EquipmentData as PassiveSonarDataSO;

            if (sonarGizmoEnabled)
            {


                // 1. Définir la couleur du Gizmo
                // Utilisation d'une couleur semi-transparente pour mieux visualiser
                Gizmos.color = passiveSonarDataSO.DetectionRangeSphereGizmoColor;
                Gizmos.DrawSphere(transform.position, passiveSonarDataSO.DetectionRange);

                // Optionnel : Dessiner le contour pour une meilleure visibilité
                Gizmos.color = passiveSonarDataSO.DetectionRangeSphereGizmoContourColor;
                Gizmos.DrawWireSphere(transform.position, passiveSonarDataSO.DetectionRange);

                // 1. Définir la couleur du Gizmo
                // Utilisation d'une couleur semi-transparente pour mieux visualiser
                Gizmos.color = passiveSonarDataSO.IdentificationRangeSphereGizmoColor;
                Gizmos.DrawSphere(transform.position, passiveSonarDataSO.DetectionRange * (1 - passiveSonarDataSO.IdentificationRatio));

                // Optionnel : Dessiner le contour pour une meilleure visibilité
                Gizmos.color = passiveSonarDataSO.IdentificationRangeSphereGizmoContourColor;
                Gizmos.DrawWireSphere(transform.position, passiveSonarDataSO.DetectionRange * (1- passiveSonarDataSO.IdentificationRatio));
            }
        }
    }    
}

