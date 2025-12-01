using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.VFX;

namespace Sonar
{
    public class SubmarineController : MonoBehaviour
    {
        [SerializeField] private SubmarineDataSO submarineData;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private EquipmentController equipmentController;
        [SerializeField] private GameObjectAndVector3EventChannelSO MoveToDestinationEvent;
        [SerializeField] private GameObjectAndVector3EventChannelSO SetDestinationEvent;
        [SerializeField] private GameObjectEventChannelSO StopEvent;
        [SerializeField] private GameObjectAndFloatEventChannelSO MoveEvent;
        [SerializeField] private GameObjectEventChannelSO DieEvent = default;
        //[SerializeField] private AudioClipInformationEventChannelSO audioClipInformationEvent;

        VisualEffect submarineExplosion;

        AudioSource audioSource;
        
        private bool isExploding = false;

        [SerializeField] private bool isInitialized = false;

        private void OnEnable()
        {
            if (MoveToDestinationEvent != null)
            {
                MoveToDestinationEvent.OnEventRaised += HandleMoveToDestinationEvent;
            }
            if (MoveEvent != null)
            {
                MoveEvent.OnEventRaised += HandleMoveEvent;
            }
            if (SetDestinationEvent != null)
            {
                SetDestinationEvent.OnEventRaised += HandleSetDestinationEvent;
            }
            if (StopEvent != null)
            {
                StopEvent.OnEventRaised += HandleStopEvent;
            }
            if (DieEvent != null)
            {
                DieEvent.OnEventRaised += HandleDieEvent;
            }
        }

        private void OnDisable()
        {
            if (MoveToDestinationEvent != null)
            {
                MoveToDestinationEvent.OnEventRaised -= HandleMoveToDestinationEvent;
            }
            if (MoveEvent != null)
            {
                MoveEvent.OnEventRaised -= HandleMoveEvent;
            }
            if (SetDestinationEvent != null)
            {
                SetDestinationEvent.OnEventRaised -= HandleSetDestinationEvent;
            }
            if (StopEvent != null)
            {
                StopEvent.OnEventRaised -= HandleStopEvent;
            }
            if (DieEvent != null)
            {
                DieEvent.OnEventRaised -= HandleDieEvent;
            }
        }

        public void Init(SubmarineDataSO submarineData)
        {
            this.submarineData = submarineData;
            if (navMeshAgent != null)
            {
                navMeshAgent.speed = submarineData.BaseSpeed;
            }
            if (!submarineData.CanMove)
            {
                navMeshAgent.isStopped = true;
            }
            isInitialized = true;
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            submarineExplosion = GetComponent<VisualEffect>();
            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();

            }
            if (equipmentController == null)
            {
                equipmentController = GetComponent<EquipmentController>();
            }
        }
        
        private void Explode()
        {
            Debug.Log("Health: Submarine is exploding.");
            //audioClipInformationEvent?.RaiseEvent(new AudioClipInformation(submarineData.ExplosionAudioClip, submarineData.ExplosionAudioClipVolume));
            audioSource.resource = submarineData.ExplosionAudioClip;
            audioSource.volume = submarineData.ExplosionAudioClipVolume;
            audioSource.Play();
            submarineExplosion?.SendEvent("OnExplode");
            Destroy(gameObject, 3.0f);
        }





        private void HandleMoveToDestinationEvent(GameObject emitter, Vector3 destination)
        {
            if (emitter != gameObject)
            {
                return;
            }
            if (!isInitialized)
            {
                Debug.Log("SubmarineController: not initialized");
                return;
            }
            if (!equipmentController.IsCurrentEngineEnabled())
            {
                Debug.Log("Current engine is not started. Please start the engine to be able to move.");
                return;
            }
            if (submarineData.CanMove)
            {
                navMeshAgent.isStopped = false;
            }
            SetDestination(destination);
        }

        public void HandleMoveEvent(GameObject emitter, float speed)
        {
            if (emitter != gameObject)
            {
                return;
            }
            if (!isInitialized)
            {
                Debug.Log("SubmarineController: not initialized");
                return;
            }
            Debug.Log($"Submarine {gameObject.name} : EquipementController = {equipmentController}");
            if (equipmentController != null && !equipmentController.IsCurrentEngineEnabled())
            {
                Debug.Log("Current engine is not started. Please start the engine to be able to move.");
                return;
            }
            if (submarineData.CanMove)
            {
                navMeshAgent.isStopped = false;
            }
            Debug.Log($"Submarine {gameObject.name} : isStopped = {navMeshAgent.isStopped}, will move with speed: {speed}");
            navMeshAgent.speed = speed;
        }

        public void Stop(GameObject emitter)
        {
            if (emitter != gameObject)
            {
                return;
            }
            if (!isInitialized)
            {
                Debug.Log("SubmarineController: not initialized");
                return;
            }
            navMeshAgent.isStopped = true;
            navMeshAgent.speed = 0;
        }

        public void HandleSetDestinationEvent(GameObject emitter, Vector3 destination)
        {
            if (emitter != gameObject)
            {
                return;
            }
            if (!isInitialized)
            {
                Debug.Log("SubmarineController: not initialized");
                return;
            }
            SetDestination(destination);
        }

        private void HandleStopEvent(GameObject emitter)
        {
            if (emitter != gameObject)
            {
                return;
            }
            if (!isInitialized)
            {
                Debug.Log("SubmarineController: not initialized");
                return;
            }
            Stop(emitter);
        }

        private void HandleDieEvent(GameObject emitter)
        {
            if (emitter != gameObject)
            {
                return;
            }
            if (!isInitialized)
            {
                Debug.Log("SubmarineController: not initialized");
                return;
            }
            Explode();
        }

    /// <summary>
        /// Gère l'ordre de mouvement donné par le joueur (ex: après un clic de souris).
        /// </summary>
        /// <param name="targetPosition">La position 3D vers laquelle le joueur veut se déplacer.</param>
        public void SetDestination(Vector3 targetPosition)
        {
            if (!isInitialized)
            {
                Debug.Log("SubmarineController: not initialized");
                return;
            }
            if (navMeshAgent == null) return;
            
            // 1. Créer un objet de chemin (Path) pour stocker le résultat du calcul.
            NavMeshPath path = new NavMeshPath();
            
            // 2. Tenter de calculer le chemin.
            // Cette méthode est plus fiable que SetDestination seule, car elle vérifie la complétude avant d'agir.
            if (navMeshAgent.CalculatePath(targetPosition, path))
            {
                // 3. Vérifier le statut du chemin.
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    // Le chemin est complet (destination valide et atteignable).
                    navMeshAgent.SetPath(path);
                    Debug.Log("Mouvement initié vers une destination valide sur le NavMesh.");
                }
                else
                {
                    // Le chemin est incomplet (PathPartial ou PathInvalid), généralement car la cible 
                    // est sur une zone non navigable (obstacle 'Not Walkable' ou hors map).
                    Debug.LogWarning("Destination cliquée invalide ou bloquée. Mouvement refusé.");
                    // L'Agent ne reçoit aucun nouvel ordre de mouvement.
                }
            }
            else
            {
                // Échec du calcul du chemin (très rare, mais sécurise le code).
                Debug.LogWarning("Échec du calcul de chemin NavMesh.");
            }
        }
    }    
}