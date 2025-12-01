using UnityEngine;
using System.Collections.Generic;
using System.Data.Common;

namespace Sonar
{
    public class EquipmentController : MonoBehaviour
    {
        [SerializeField] private EquipmentsListSO equipments;

        [SerializeField] private PassiveSonar currentPassiveSonar;
        [SerializeField] private ActiveSonar currentActiveSonar;
        [SerializeField] private SimpleEngine currentEngine;

        [SerializeField] private GameObjectEventChannelSO UseActiveSonarEvent;

        [SerializeField] private GameObjectEventChannelSO ToggleEngineEvent;

        [SerializeField] private BoolVariableSO engineEnabledVariable;

        public float SoundIntensity;
        public float DetectionIntensity;

        [SerializeField] private bool isInitialized = false;

        private void OnEnable()
        {
            //inputReader.EngineEvent += OnEngineEvent;
            if (UseActiveSonarEvent != null)
            {
                UseActiveSonarEvent.OnEventRaised += HandleSonarEvent;
            }
            if (ToggleEngineEvent != null)
            {
                ToggleEngineEvent.OnEventRaised += HandleEngineEvent;
            }
        }

        private void OnDisable()
        {
            //inputReader.EngineEvent -= OnEngineEvent;
            if (UseActiveSonarEvent != null)
            {
                UseActiveSonarEvent.OnEventRaised -= HandleSonarEvent;
            }
            if (ToggleEngineEvent != null)
            {
                ToggleEngineEvent.OnEventRaised -= HandleEngineEvent;
            }
        }

        public void HandleEngineEvent(GameObject eventSource)
        {
            Debug.Log($"EquipmentController ({gameObject}): Engine event received from {eventSource.name}.");
            if(eventSource == gameObject)
            {
                Debug.Log($"Equipment controller {gameObject.name} is currently handling an engine event from {eventSource.name}");
                if(currentEngine != null)
                {
                    currentEngine.ToggleEngine();
                }
            }
        }

        public void HandleSonarEvent(GameObject eventSource)
        {
            Debug.Log($"EquipmentController ({gameObject}): Sonar event received from {eventSource.name}.");
            if(eventSource == gameObject)
            {
                Debug.Log($"Equipment controller {gameObject.name} is currently handling a sonar event from {eventSource.name}");
                currentActiveSonar.Behave(this);
            }
        }

        public void Init(EquipmentsListSO startEquipements)
        {
            this.equipments = startEquipements;
            isInitialized = true;
        }

        private void Start()
        {
            GlobalSonarContext globalSonarContext = GetComponent<GlobalSonarContext>();
            if(equipments != null)
            {
                if (equipments.PassiveSonar != null && equipments.PassiveSonar.EquipmentPrefab != null)
                {
                    Instantiate(equipments.PassiveSonar.EquipmentPrefab, transform).TryGetComponent<PassiveSonar>(out currentPassiveSonar);
                    if (currentPassiveSonar != null)
                    {
                        currentPassiveSonar.Init(this, equipments.PassiveSonar, globalSonarContext.GlobalSonarContextSO);
                    }
                }
                if (equipments.ActiveSonar != null && equipments.ActiveSonar.EquipmentPrefab != null)
                {
                    Instantiate(equipments.ActiveSonar.EquipmentPrefab, transform).TryGetComponent<ActiveSonar>(out currentActiveSonar);
                    if (currentActiveSonar != null)
                    {
                        currentActiveSonar.Init(this, equipments.ActiveSonar);
                    }
                }
                if (equipments.SimpleEngine != null && equipments.SimpleEngine.EquipmentPrefab != null)
                {
                    Instantiate(equipments.SimpleEngine.EquipmentPrefab, transform).TryGetComponent<SimpleEngine>(out currentEngine);
                    if (currentEngine != null)
                    {
                        currentEngine.Init(this, equipments.SimpleEngine);
                    }
                }                
            }
        }

        public bool IsCurrentEngineEnabled()
        {
            Debug.Log($"Current engine: {currentEngine == null}");
            if(currentEngine == null)
            {
                Debug.Log("Current engine is not already initialized.");
                return false;
            }
            return currentEngine.IsStarted;
        }

        private void Update()
        {
            if (isInitialized)
            {
                currentPassiveSonar?.Behave(this);
            }
        }
    }    
}
