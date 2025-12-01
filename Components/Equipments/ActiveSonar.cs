using System.Runtime.CompilerServices;
using UnityEngine;

namespace Sonar
{
    public class ActiveSonar : BaseEquipment
    {
        [SerializeField] private DetectionInformationEventChannelSO identificationInformationEvent;
        [SerializeField] private SoundInformationEventChannelSO sonarSoundEvent;

        [SerializeField] private bool sonarGizmoEnabled = false;

        AudioSource audioSource;
        public ActiveSonarDataSO ActiveSonarData;

        [SerializeField] private SimpleSonarWave simpleSonarWave;

        private float pingTimer;

        public void Init(EquipmentController theEquipmentController, ActiveSonarDataSO activeSonarData)
        {
            this.ActiveSonarData = activeSonarData;
            sonarGizmoEnabled = ActiveSonarData.IsGizmoEnabled;
            if (audioSource != null)
            {
                Debug.Log("Active Sonar: setting audio clip and volume.");
                audioSource.resource = ActiveSonarData.AudioClip;
                audioSource.volume = ActiveSonarData.AudioClipVolume;
            }
            base.Init(theEquipmentController, ActiveSonarData);
        }
        private void Awake()
        {

            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            //Debug.Log($"Origin: {transform.position} | Radius: {currentRadius}");
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {

        }

        private void Start()
        {
            //Debug.Log($"Active Sonar '{EquipmentData.EquipmentName}' is now active.");
            pingTimer = 0f; // Initialize timer to ping immediately upon enabling
        }

        public override void Behave(EquipmentController EquipmentController)
        {
            if (!IsInitialized)
            {
                Debug.Log("Active Sonar: was not initialized.");
                return;
            }
            Debug.Log($"Active Sonar: Active Sonar from '{EquipmentData.EquipmentName}' pinged.");
            if (EquipmentData != null)
            {
                audioSource?.Play();
                if (ActiveSonarData.VisualEffectEnabled)
                {
                    simpleSonarWave?.FireSonar();
                }
                pingTimer -= Time.deltaTime;
                if (pingTimer <= 0f)
                {
                    PingForObjects(EquipmentController);
                    pingTimer = ActiveSonarData.CoolDown; // Reset timer
                    Debug.Log($"Active Sonar: Active Sonar '{EquipmentData.EquipmentName}' from {EquipmentController.gameObject} is producing sound with intensity {ActiveSonarData.SoundIntensity}.");
                        sonarSoundEvent.RaiseEvent(new SoundInformation(
                            EquipmentController.gameObject,
                            EquipmentController.transform.position,
                            null,
                            ActiveSonarData.SoundIntensity));
                }
                else
                {
                    Debug.Log($"Active Sonar: Active Sonar '{ActiveSonarData.EquipmentName}' cooling down. Time remaining: {pingTimer} seconds.");
                }


            }
        }
        
        private void PingForObjects(EquipmentController equipmentController)
        {
            // Logic to detect objects within detectionRange
            Debug.Log($"Active Sonar: Ping! Detecting objects within {ActiveSonarData.DetectionRange} units.");
            // Example: Use Physics.OverlapSphere to find nearby objects
            Ray ray = new Ray(equipmentController.transform.position, Vector3.forward);

            Collider[] hits = Physics.OverlapSphere(equipmentController.transform.position, ActiveSonarData.DetectionRange, ActiveSonarData.DetectionMask);
            bool foundAnyTarget = false;

            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    EquipmentController targetequipment = hit.GetComponent<EquipmentController>();
                    if (targetequipment != null && targetequipment != equipmentController)
                    {
                        Debug.Log($"Active Sonar: Detected object: {hit.gameObject.name} at distance {Vector3.Distance(equipmentController.transform.position, hit.transform.position)}");
                        Debug.Log("Active Sonar: Identification!");
                ActiveSonar activeSonar = equipmentController.GetComponent<ActiveSonar>();
                identificationInformationEvent.RaiseEvent(new DetectionInformation(
                        hit.gameObject,
                        hit.gameObject.transform.position,
                        1.0f,
                        null,
                        equipmentController));
                        foundAnyTarget = true;
                    }
                }
            }

            if (!foundAnyTarget)
            {
                Debug.Log("Active Sonar: No objects detected within sonar range.");
            }
        }

        private void OnDrawGizmos()
        {
            // Vérifie si le ScriptableObject est assigné
            if (EquipmentData == null)
            {
            Debug.Log("Equipment is null.");
                return;
            }

            if (!sonarGizmoEnabled)
            {
                return;
            }

            // 2. Dessiner la sphère pleine (ou DrawWireSphere pour un contour)
            // La position est celle du GameObject. Le rayon est dans le ScriptableObject.
            ActiveSonarDataSO ActiveSonarData = EquipmentData as ActiveSonarDataSO;
            // 1. Définir la couleur du Gizmo
            // Utilisation d'une couleur semi-transparente pour mieux visualiser
            Gizmos.color = ActiveSonarData.DetectionRangeSphereGizmoColor; // Bleu-cyan transparent
            Gizmos.DrawSphere(transform.position, ActiveSonarData.DetectionRange);

            // Optionnel : Dessiner le contour pour une meilleure visibilité
            Gizmos.color = ActiveSonarData.DetectionRangeSphereGizmoContourColor;
            Gizmos.DrawWireSphere(transform.position, ActiveSonarData.DetectionRange);
        }
    }    
}
