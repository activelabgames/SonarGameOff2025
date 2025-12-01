using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Sonar
{
    public class SimpleEngine : BaseEquipment
    {
        private NavMeshAgent navMeshAgent;

        [SerializeField] private SoundInformationEventChannelSO engineSoundEvent;
        [SerializeField] private SimpleEngineDataSO simpleEngineData;
        [SerializeField] private float currentSoundIntensity = 0.0f;

        [SerializeField] private GameObjectEventChannelSO ControllerDestroyedEvent;

        [SerializeField] private bool isStarted;

        public bool IsStarted => isStarted;

        AudioSource audioSource;

        private void OnEnable()
        {
            if (ControllerDestroyedEvent != null)
            {
                ControllerDestroyedEvent.OnEventRaised += HandleControllerDestroyedEvent;
            }
        }

        private void OnDisable()
        {
            if (ControllerDestroyedEvent != null)
            {
                ControllerDestroyedEvent.OnEventRaised -= HandleControllerDestroyedEvent;
            }
        }

        private void HandleControllerDestroyedEvent(GameObject destroyedController)
        {
            Debug.Log($"Engine: Received ControllerDestroyedEvent for {destroyedController.name}");
            if(EquipmentController != null && destroyedController == EquipmentController.gameObject)
            {
                Debug.Log($"Engine: Detected destruction of EquipmentController {destroyedController.name}, stopping engine sound.");
                isStarted = false;
            }
        }
        public void Init(EquipmentController theEquipmentController, SimpleEngineDataSO simpleEngineDataSO)
        {
            this.simpleEngineData = simpleEngineDataSO;
            if (audioSource != null)
            {
                Debug.Log("Engine: setting audio clip and volume.");
                audioSource.resource = simpleEngineData.AudioClip;
                audioSource.volume = simpleEngineData.AudioVolume;
            }
            Debug.Log($"SimpleEngine: toggling engine to: {isStarted}");
            isStarted = simpleEngineData.IsStarted;
            if (simpleEngineData.EngineEnabledVariable != null)
            {
                simpleEngineData.EngineEnabledVariable.Value = isStarted;                
            }
            ToggleEngine(isStarted);
            base.Init(theEquipmentController, simpleEngineDataSO);
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        private void Start()
        {
            simpleEngineData = EquipmentData as SimpleEngineDataSO;
            if (EquipmentController != null)
            {
                navMeshAgent = EquipmentController.GetComponent<NavMeshAgent>();
            }
            ToggleEngine(isStarted);
        }

        private void Update()
        {
            if (!IsInitialized)
            {
                Debug.Log("Simple Engine: was not initialized.");
                return;
            }
            Debug.Log($"Velocity magnitude: {navMeshAgent.velocity.magnitude}");
            if (isStarted)
            {
                if (navMeshAgent != null)
                {
                    currentSoundIntensity = navMeshAgent.velocity.magnitude * simpleEngineData.SoundIntensity + simpleEngineData.SoundIntensity;
                }                
            }
            else
            {
                currentSoundIntensity = 0.0f;
            }


            if (simpleEngineData != null && currentSoundIntensity > 0)
            {
                Debug.Log($"Simple Engine '{EquipmentData.EquipmentName}' from {EquipmentController.gameObject} is producing sound with intensity {simpleEngineData.SoundIntensity}.");
                engineSoundEvent.RaiseEvent(new SoundInformation(
                    EquipmentController.gameObject,
                    EquipmentController.transform.position,
                    null,
                    currentSoundIntensity));
            }
            /*if (wasStarted != isStarted)
            {
                ToggleEngine(isStarted);
            }*/
            float currentVolume = currentSoundIntensity / (simpleEngineData.SoundIntensity * 10.0f) ;
            audioSource.volume = Mathf.Clamp(currentVolume, 0.0f, 1.0f);
            ConfigureEngineSound();
            //wasStarted = isStarted;
            //Debug.Log($"NavMeshAgent Velocity: {navMeshAgent.velocity.magnitude}");
        }

        private void ConfigureEngineSound()
        {
            if (isStarted)
            {
                Debug.Log("Engine: starting engine sound");
                if (!simpleEngineData.IsMute)
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();   
                    }
                }
            }
            else
            {
                Debug.Log("Engine: stopping engine sound");
                if (audioSource.isPlaying)
                {
                    
                    audioSource.Stop();
                }
            }
        }

        public void ToggleEngine(bool started)
        {
            bool wasStarted = isStarted;
            Debug.Log($"Engine: Toggling engine to {(started ? "started" : "stopped")}");
            isStarted = started;
            if (simpleEngineData.EngineEnabledVariable != null)
            {
                Debug.Log($"Changing engineenabled variable to {isStarted}");
                simpleEngineData.EngineEnabledVariable.Value = isStarted;                
            }

            if(wasStarted && !isStarted)
            {
                if (simpleEngineData != null)
                {
                    Debug.Log($"Simple Engine '{EquipmentData.EquipmentName}' from {EquipmentController.gameObject} is producing sound with intensity {simpleEngineData.SoundIntensity}.");
                    engineSoundEvent.RaiseEvent(new SoundInformation(
                        EquipmentController.gameObject,
                        EquipmentController.transform.position,
                        null,
                        0));
                }
            }
        }

        public void ToggleEngine()
        {
            bool wasStarted = isStarted;
            Debug.Log($"Engine: Toggling engine state to {!isStarted}");
            isStarted = !isStarted;
            if (simpleEngineData.EngineEnabledVariable != null)
            {
                Debug.Log($"Changing engineenabled variable to {isStarted}");
                simpleEngineData.EngineEnabledVariable.Value = isStarted;                
            }

            if(wasStarted && !isStarted)
            {
                if (simpleEngineData != null)
                {
                    Debug.Log($"Simple Engine '{EquipmentData.EquipmentName}' from {EquipmentController.gameObject} is producing sound with intensity {simpleEngineData.SoundIntensity}.");
                    engineSoundEvent.RaiseEvent(new SoundInformation(
                        EquipmentController.gameObject,
                        EquipmentController.transform.position,
                        null,
                        0));
                }
            }
        }
    }
}
