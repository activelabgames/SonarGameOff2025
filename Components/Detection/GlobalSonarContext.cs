using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sonar
{
    public class GlobalSonarContext : MonoBehaviour
    {
        [Header("Echo Configuration")]
        [SerializeField] private EchoTypeSO unknownEcho;
        [SerializeField] private EchoTypeSO identifiedEcho;
        [SerializeField] private EchoTypeSO activelyIdentifiedEcho;
        [SerializeField] private EchoTypeSO lostEcho;
        [SerializeField] private GlobalSonarContextSO globalSonarContextSO;
        public GlobalSonarContextSO GlobalSonarContextSO => globalSonarContextSO;

        // Sonar Policy
        [SerializeField] private SonarPolicySO currentSonarPolicy;

        [Header("Events")]
        [SerializeField] DetectionInformationEventChannelSO detectionInformationEvent;
        [SerializeField] DetectionInformationEventChannelSO identificationInformationEvent;
        [SerializeField] private DetectionInformationEventChannelSO lostInformationEvent;
        [SerializeField] EchoEventChannelSO echoEvent;
        [SerializeField] private GameObjectEventChannelSO useActiveSonarEvent;
        [SerializeField] GameObjectEventChannelSO representationDestroyedEvent;

        [Header("Runtime Data")]
        // On utilise DetectableObject (le script) comme clé plutôt que le GameObject pour éviter des GetComponent inutiles
        private Dictionary<GameObject, EchoData> trackedEchoes = new Dictionary<GameObject, EchoData>();
        
        // Buffers
        private List<DetectionInformation> detectionsBuffer = new List<DetectionInformation>();
        private List<DetectionInformation> identificationsBuffer = new List<DetectionInformation>();
        private List<DetectionInformation> lostsBuffer = new List<DetectionInformation>();

        private float sonarPolicyTimer = 0.0f;
        private float detectionPausedTimer = 0.0f;

        private bool isPlayer = false;

        AudioSource audioSource;
        private AudioFaderCurve audioFaderCurve;

        [SerializeField] private bool isInitialized = false;

        // Classe interne pour stocker l'état enrichi (Data Fusion)
        private class EchoData
        {
            public Echo EchoInfo; // L'objet Echo standard transmis aux autres systèmes
            public GameObject Representation; // L'instance visuelle
            public float LastHighPriorityTime; // Quand a-t-on reçu la dernière info "Active" ?
        }

        private void OnEnable()
        {
            detectionInformationEvent.OnEventRaised += HandleDetectionInformationEvent;
            identificationInformationEvent.OnEventRaised += HandleIdentificationInformationEvent;
            lostInformationEvent.OnEventRaised += HandleLostInformationEvent;
            representationDestroyedEvent.OnEventRaised += HandleRepresentationDestroyedEvent;
        }

        private void OnDisable()
        {
            detectionInformationEvent.OnEventRaised -= HandleDetectionInformationEvent;
            identificationInformationEvent.OnEventRaised -= HandleIdentificationInformationEvent;
            lostInformationEvent.OnEventRaised -= HandleLostInformationEvent;
            representationDestroyedEvent.OnEventRaised -= HandleRepresentationDestroyedEvent;
        }

        public void Init(GlobalSonarContextSO globalSonarContextSO)
        {
            this.globalSonarContextSO = globalSonarContextSO;
            currentSonarPolicy = this.globalSonarContextSO.SonarPolicy;
            isInitialized = true;
            if (audioSource != null)
            {
                Debug.Log("Passive Sonar: setting audio clip and volume.");
                audioSource.resource = globalSonarContextSO.AudioClip;
                audioSource.volume = globalSonarContextSO.AudioClipVolume;
            }
        }

        private void Awake()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            audioFaderCurve = audioSource.GetComponent<AudioFaderCurve>();
            //audioSource.ignoreListenerPause = true;
        }

        private void Start()
        {
            if (TryGetComponent(out PlayerController playerController))
            {
                isPlayer = true;
            }
        }

        private void Update()
        {
            if (!isInitialized)
            {
                Debug.Log("Global sonar context is not initialized");
                return;
            }
            HandleActiveSonarPolicy();
            
            // Batch processing des événements
            detectionPausedTimer += Time.deltaTime;
            if (detectionPausedTimer > globalSonarContextSO.DetectionPauseTime)
            {
                ConsumeEvents();
                detectionPausedTimer = 0.0f;
            }

            HandleSonarSound();
        }

        private void HandleSonarSound()
        {

            if(!isPlayer)
            {
                return;
            }

            if (audioSource != null && audioFaderCurve != null && !globalSonarContextSO.IsMute)
            {
                Debug.Log($"GlobalSonarContext: Playing echo sound at volume");
            }
            else
            {
                Debug.Log("GlobalSonarContext: Something went wrong.");
                return;
            }
            if (trackedEchoes.Count > 0)
            {
                Debug.Log("Echoes number is not null. Fading in.");
                audioFaderCurve.FadeIn();
            }
            else
            {
                Debug.Log("Echoes number is null. Fading out.");
                audioFaderCurve.FadeOut();
            }
        }

        private void PlayEchoSound(float volume)
        {

        }

        private void HandleActiveSonarPolicy()
        {
            if (sonarPolicyTimer >= currentSonarPolicy.ScanFrequency)
            {
                if (currentSonarPolicy.RegularlyScanning)
                {
                    useActiveSonarEvent.RaiseEvent(gameObject);
                }
                sonarPolicyTimer = 0.0f;
            }
            sonarPolicyTimer += Time.deltaTime;
        }

        // --- Event Handlers (Remplissage des buffers) ---
        private void HandleDetectionInformationEvent(DetectionInformation info) => detectionsBuffer.Add(info);
        private void HandleIdentificationInformationEvent(DetectionInformation info) => identificationsBuffer.Add(info);
        private void HandleLostInformationEvent(DetectionInformation info) => lostsBuffer.Add(info);
        
        // --- Core Logic ---

        private void ConsumeEvents()
        {
            // 1. Traiter les Détections (Unknown)
            foreach (var info in detectionsBuffer)
            {
                ProcessEchoUpdate(info, unknownEcho);
            }
            detectionsBuffer.Clear();

            // 2. Traiter les Identifications (Passive Identified ou Active)
            foreach (var info in identificationsBuffer)
            {
                // Détermination du type basé sur la source
                EchoTypeSO targetType = identifiedEcho; // Par défaut : passif identifié
                Debug.Log($"GlobalSonarContext {gameObject.name}: consuming event {info.ToString()}");
                // TODO: Améliorer la détection du sonar actif (évite le cast si possible via un Type dans DetectionInformation)
                if (!(info.SourceSonar is PassiveSonar)) 
                {
                    targetType = activelyIdentifiedEcho;
                }
                
                ProcessEchoUpdate(info, targetType);
            }
            identificationsBuffer.Clear();

            // 3. Traiter les Pertes
            foreach (var info in lostsBuffer)
            {
                ProcessEchoLoss(info);
            }
            lostsBuffer.Clear();
        }

        private void LogCurrentEchoes()
        {
            foreach (EchoData echoData in trackedEchoes.Values)
            {
                Debug.Log(echoData.EchoInfo.toString());
            }
        }

        /// <summary>
        /// Cœur du système de fusion de données. Décide si on met à jour, upgrade ou ignore le type.
        /// </summary>
        private void ProcessEchoUpdate(DetectionInformation info, EchoTypeSO incomingType)
        {
            // Vérifications initiales
            if (info.DetectedObject == null) return;
            if (ShouldIgnoreSource(info)) return; // Vérifie que l'écho vient du bon contrôleur

            GameObject targetObj = info.DetectedObject;

            LogCurrentEchoes();

            // 1. Récupérer ou Créer la donnée locale (EchoData)
            if (!trackedEchoes.TryGetValue(targetObj, out EchoData data))
            {
                data = new EchoData();
                trackedEchoes[targetObj] = data;
            }

            // --- 2. Logique de Priorité (Data Fusion) ---
            EchoTypeSO currentType = data.EchoInfo?.EchoType;
            EchoTypeSO finalType = incomingType;
            Debug.Log($"Current echo type: {currentType}");
            Debug.Log($"Incoming echo type: {incomingType}");

            int incomingPriority = GetTypePriority(incomingType);
            int currentPriority = GetTypePriority(currentType);
            Debug.Log($"Current echo priority: {currentPriority}");
            Debug.Log($"Incoming echo priority: {incomingPriority}");
            
            // Durée de persistance des contacts Actifs/High Confidence, à configurer dans le ScriptableObject
            float activePersistance = globalSonarContextSO.ActiveEchoPersistenceTime; 

            // A. Si l'information entrante est de qualité inférieure à l'information actuelle (Tente de Downgrader)
            if (currentPriority > incomingPriority)
            {
                // On applique la persistance UNIQUEMENT si l'état actuel est le plus critique (Priorité 3 : activelyIdentifiedEcho).
                if (currentType == activelyIdentifiedEcho)
                {
                    if (Time.time - data.LastHighPriorityTime < activePersistance)
                    {
                        // L'écho Actif/Ping est encore frais : On garde le type P3 et on met à jour la position.
                        finalType = currentType;
                    }
                    else
                    {
                        // L'écho Actif/Ping est périmé : On accepte le downgrade vers le type entrant.
                        finalType = incomingType; 
                    }
                }
                else 
                {
                    // Si l'état actuel est Priorité 2 (Identified) et l'entrant est Priorité 1 (Unknown),
                    // on accepte le downgrade IMMÉDIATEMENT (pour la torpille du joueur, par exemple).
                    finalType = incomingType;
                }
            }
            // B. Si l'information entrante est de qualité égale ou supérieure
            else if (incomingPriority >= currentPriority)
            {
                // On accepte la nouvelle information et le type associé
                finalType = incomingType;

                // Activation du timer de persistance pour les événements de haute confiance (Priorité 2 ou 3).
                if (incomingPriority >= 2) 
                {
                    data.LastHighPriorityTime = Time.time;
                }
            }
            // C. Si l'écho est nouveau (data.EchoInfo == null), le type entrant est déjà le finalType.

            // 3. Création de l'objet Echo mis à jour
            Echo newEcho = new Echo(
                this, 
                targetObj, 
                finalType, 
                0.0f, // Remplacer par info.DopplerValue si disponible
                info.DetectionLocation, 
                1 // Remplacer par info.Confidence si disponible
            );
            PlayEchoSound(info.DetectionIntensity);

            // Détection de changement pour mise à jour visuelle (Instantiate/Destroy)
            bool typeChanged = (data.EchoInfo == null) || (data.EchoInfo.EchoType != finalType);
            
            data.EchoInfo = newEcho; // Sauvegarde de la nouvelle donnée
            echoEvent.RaiseEvent(newEcho); // Diffusion globale

            // 4. Gestion de la Représentation Visuelle (qui appelle Refresh ou Instantiate/Destroy)
            UpdateRepresentation(data, typeChanged);
        }

        private void ProcessEchoLoss(DetectionInformation info)
        {
            if (info.DetectedObject == null || ShouldIgnoreSource(info)) return;

            if (trackedEchoes.TryGetValue(info.DetectedObject, out EchoData data))
            {
                // Nettoyage visuel immédiat
                if (data.Representation != null)
                {
                    Destroy(data.Representation);
                }
                
                // Envoi de l'event "Lost"
                Echo lostEchoData = new Echo(this, info.DetectedObject, lostEcho, 0f, info.DetectionLocation, 0);
                echoEvent.RaiseEvent(lostEchoData);

                trackedEchoes.Remove(info.DetectedObject);
            }
        }

        private void UpdateRepresentation(EchoData data, bool typeChanged)
        {
            // Uniquement pour le joueur humain (si pas d'IA)
            if (TryGetComponent<AIController>(out AIController ai)) return;

            // CAS 1 : NOUVEL ECHO ou CHANGEMENT DE TYPE (Ex: Inconnu -> Identifié)
            if (typeChanged || data.Representation == null)
            {
                // 1. Nettoyage de l'ancien visuel s'il existe
                if (data.Representation != null)
                {
                    Destroy(data.Representation);
                }

                // 2. Choix du bon prefab
                GameObject prefabToSpawn = GetPrefabForType(data.EchoInfo);
                
                if (prefabToSpawn != null && data.EchoInfo.DetectedObject != null)
                {
                    // 3. Instanciation
                    GameObject newRep = Instantiate(
                        prefabToSpawn, 
                        data.EchoInfo.LastPosition + new Vector3(0f, 0.5f, 0f), 
                        Quaternion.identity // Ou data.EchoInfo.DetectedObject.transform.rotation si tu veux la rotation réelle
                    );
                    
                    // 4. Configuration initiale du composant
                    if (newRep.TryGetComponent<EchoComponent>(out EchoComponent comp))
                    {
                        comp.Lifetime = globalSonarContextSO.EchoLifetime;
                        comp.DetectedObject = data.EchoInfo.DetectedObject;
                        // Pas strictement nécessaire si Start() le fait, mais plus sûr :
                        comp.Refresh(); 
                    }
                    
                    data.Representation = newRep;
                }
            }
            // CAS 2 : MÊME TYPE, JUSTE UNE MISE À JOUR DE POSITION
            else
            {
                if (data.Representation != null)
                {
                    // A. Déplacement fluide (au lieu de détruire/créer)
                    data.Representation.transform.position = data.EchoInfo.LastPosition + new Vector3(0f, 0.5f, 0f);
                    
                    // B. LE POINT CRUCIAL QUE J'AVAIS OUBLIÉ :
                    // On remet le timer à zéro pour que l'écho ne disparaisse pas tant qu'on le détecte
                    if (data.Representation.TryGetComponent<EchoComponent>(out EchoComponent comp))
                    {
                        comp.Refresh(); 
                    }
                }
            }
        }

        // --- Helpers ---

        private int GetTypePriority(EchoTypeSO type)
        {
            if (type == activelyIdentifiedEcho) return 3;
            if (type == identifiedEcho) return 2;
            if (type == unknownEcho) return 1;
            return 0;
        }

        private GameObject GetPrefabForType(Echo echo)
        {
            if (echo.DetectedObject == null) return null;
            
            // On récupère le DetectableObject une fois
            if (!echo.DetectedObject.TryGetComponent<DetectableObject>(out DetectableObject detectable)) 
                return null;

            if (echo.EchoType == identifiedEcho || echo.EchoType == activelyIdentifiedEcho)
            {
                return detectable.IdentifiedPrefab;
            }
            return detectable.UnknownPrefab;
        }

        private bool ShouldIgnoreSource(DetectionInformation info)
        {
            return info.SourceEquipmentController != null && info.SourceEquipmentController.gameObject != gameObject;
        }

        private void HandleRepresentationDestroyedEvent(GameObject emittingObject)
        {
            // Nettoyage si un EchoComponent se détruit tout seul (timeout)
            if (emittingObject.TryGetComponent<EchoComponent>(out EchoComponent echoComp))
            {
                if (echoComp.DetectedObject != null && trackedEchoes.ContainsKey(echoComp.DetectedObject))
                {
                    // Attention: ici on ne veut peut-être pas supprimer les DATA, juste la REF visuelle
                    // Si le code original voulait tout supprimer, on laisse comme ça :
                    trackedEchoes.Remove(echoComp.DetectedObject);
                }
            }
            Destroy(emittingObject);
        }
    }
}