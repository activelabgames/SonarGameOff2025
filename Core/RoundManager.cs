using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

using Sonar.AI;

namespace Sonar
{
    public class RoundManager : MonoBehaviour
    {
        [SerializeField] private RoundConfigurationSO roundConfigurationSO;
        [SerializeField] private RoundParametersSO roundParametersSO;
        [SerializeField] private bool standaloneRound = false;
        [SerializeField] private GameParametersSO gameParametersSO;
        [SerializeField] private GameDataSO gameDataSO;
        [SerializeField] private LayerMask clickableLayer;
        [SerializeField] private GameObject waterSurface; // L'objet utilisé pour déterminer la taille de la zone
        [SerializeField] private GameObjectVariableSO playerSubmarinePrefabTD;
        [SerializeField] private Transform playerSpawnPoint;

        [SerializeField] private GameObjectVariableSO enemySubmarinePrefabTD;
        [SerializeField] private Transform ennemySpawnPoint;

        [SerializeField] private WeaponsSetSO playerCurrentWeaponsListTD;
        [SerializeField] private WeaponsSetSO enemyCurrentWeaponsListTD;

        [SerializeField] private GameObject waypointPrefab;

        [SerializeField] private Image playerDiscretionBar;
        [SerializeField] private Image[] playerDiscretionPoints;

        [SerializeField] private GameMessageEventChannelSO gameMessageEvent;
        [SerializeReference] private EmptyEventChannelSO timeoutEvent;
        [SerializeField] BoolEventChannelSO pauseEvent;
        [SerializeField] EmptyEventChannelSO messageEndEvent;
        [SerializeField] EmptyEventChannelSO displayScoreEvent;
        [SerializeField] GameObjectEventChannelSO dieEvent;
        [SerializeField] private EmptyEventChannelSO roundEndedEvent;
        [SerializeField] private ScoreEventChannelSO scoreEvent;

        [SerializeField] CameraController cameraController;

        private List<GameObject> playerInstances = new List<GameObject>();

        [SerializeField] private PlayerController playerController;

        private bool isScoreDisplayPending = false;


        private void OnEnable()
        {
            timeoutEvent.OnEventRaised += HandleTimeoutEvent;
            pauseEvent.OnEventRaised += HandlePauseEvent;
            dieEvent.OnEventRaised += HandleDieEvent;
        }

        private void OnDisable()
        {
            timeoutEvent.OnEventRaised -= HandleTimeoutEvent;
            pauseEvent.OnEventRaised -= HandlePauseEvent;
            dieEvent.OnEventRaised -= HandleDieEvent;
        }

        private void Awake()
        {
            if (gameDataSO == null)
            {
                Debug.LogError("RoundManager: GameDataSO is not assigned!");
            }

            if (roundConfigurationSO != null && roundParametersSO != null && standaloneRound)
            {
                roundConfigurationSO.RoundDuration.Value = roundParametersSO.RoundDuration;
                roundConfigurationSO.UnlimitedTime = roundParametersSO.UnlimitedTime;
                roundConfigurationSO.EnemiesNumber = roundParametersSO.EnemiesNumber;

                enemyCurrentWeaponsListTD.PrimaryWeaponAmmo.Value = gameParametersSO.PlayersParameters.EnemyParameters.StartWeapons.PrimaryWeaponAmmo.Value;
                playerCurrentWeaponsListTD.PrimaryWeaponAmmo.Value = gameParametersSO.PlayersParameters.PlayerParameters.StartWeapons.PrimaryWeaponAmmo.Value;
            }
            else
            {
                if (roundConfigurationSO != null)
                {
                    gameParametersSO = roundConfigurationSO.GameParameters;
                }
            }

            gameDataSO.RemainingEnemies.Value = roundConfigurationSO.EnemiesNumber;
        }

        private void Start()
        {
            isScoreDisplayPending = false;
            messageEndEvent.OnEventRaised += HandleMessageEndEventVariant1;
            gameMessageEvent?.RaiseEvent(new GameMessage($"Round {gameDataSO.CurrentRoundIndex.Value}", true, 3.0f));
        }

        private void HandleMessageEndEventVariant1()
        {
            messageEndEvent.OnEventRaised -= HandleMessageEndEventVariant1;
            CreatePlayer();
            MoveCameraToPlayer();

            gameDataSO.EnemyData.PlayerName.Value = "Enemy";
            gameDataSO.EnemyData.PlayerScore.Player = gameDataSO.EnemyData;

            for (int i = 0; i < roundConfigurationSO.EnemiesNumber; i++)
            {
                SpawnEnemy(i);
            }
        }


        private void CreatePlayer()
        {
            Vector3 playerSpawnPosition = Vector3.zero;
            // DYNAMIQUE: Récupère le rayon maximal de la carte
            float mapRange = GetMapMaxRadius(waterSurface);

            if (playerSpawnPoint == null)
            {
                // Tente de trouver un point NavMesh aléatoire sur toute la zone
                if (!RandomPoint(waterSurface.transform.position, mapRange, out playerSpawnPosition))
                {
                    // Fallback 1: Tente un échantillonnage plus large au centre
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(waterSurface.transform.position, out hit, 50.0f, NavMesh.AllAreas))
                    {
                        playerSpawnPosition = hit.position;
                        Debug.LogWarning("Fallback: NavMesh position for player found using a wide search at the center.");
                    }
                    else
                    {
                        // Fallback 2: NavMesh introuvable
                        Debug.LogError("FATAL ERROR: Failed to find NavMesh position for player spawn. Falling back to Vector3.zero.");
                        playerSpawnPosition = Vector3.zero;
                    }
                }
            }
            else
            {
                // Si un point de spawn a été assigné
                playerSpawnPosition = playerSpawnPoint.position;
            }


            // Instantiate the player submarine at the start of the game
            GameObject playerSubmarine = Instantiate(gameParametersSO.PlayersParameters.PlayerParameters.SubmarineData.Prefab, playerSpawnPosition, Quaternion.identity) as GameObject;
            playerSubmarine.name = "PlayerSubmarine";
            // Player controller
            playerController = playerSubmarine.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ClickableLayer = clickableLayer;
            }

            // Submarine controller
            playerSubmarine.TryGetComponent<SubmarineController>(out SubmarineController submarineController);
            if (submarineController != null)
            {
                Debug.Log("RoundManager: initializing player submarinecontroller");
                submarineController.Init(gameParametersSO.PlayersParameters.PlayerParameters.SubmarineData);
            }

            // Global sonar context
            playerSubmarine.TryGetComponent<GlobalSonarContext>(out GlobalSonarContext globalSonarContext);
            if (globalSonarContext != null)
            {
                globalSonarContext.Init(gameParametersSO.PlayersParameters.PlayerParameters.GlobalSonarContext);
            }

            // Health controller
            playerSubmarine.TryGetComponent<HealthComponent>(out HealthComponent healthController);
            if (healthController != null)
            {
                healthController.Init(gameParametersSO.PlayersParameters.PlayerParameters.HealthCharacteristics);
            }

            // EquipmentsController
            playerSubmarine.TryGetComponent<EquipmentController>(out EquipmentController equipmentController);
            if (equipmentController != null)
            {
                equipmentController.Init(gameParametersSO.PlayersParameters.PlayerParameters.StartEquipments);
            }

            // WeaponsController
            playerSubmarine.TryGetComponent<WeaponsController>(out WeaponsController weaponsController);
            if (weaponsController != null)
            {
                weaponsController.Init(gameParametersSO.PlayersParameters.PlayerParameters.StartWeapons);
                weaponsController.PlayerController = playerController;
            }

            // DiscretionController
            playerSubmarine.TryGetComponent<DiscretionController>(out DiscretionController discretionController);

            if (discretionController != null)
            {
                discretionController.SetDiscretionBar(playerDiscretionBar);
                discretionController.SetDiscretionPoints(playerDiscretionPoints);
            }
            playerInstances.Add(playerSubmarine);
        }

        private void MoveCameraToPlayer()
        {
            if (cameraController != null && playerController != null)
            {
                cameraController.transform.position = playerController.transform.position;
            }
        }

        private void CleanRoundArea()
        {
            foreach (GameObject obj in playerInstances)
            {
                Destroy(obj);
            }
        }

        private void SpawnEnemy(int index = 1)
        {
            Vector3 ennemySpawnPosition = Vector3.zero;
            // DYNAMIQUE: Récupère le rayon maximal de la carte
            float mapRange = GetMapMaxRadius(waterSurface);
            int maxAttempts = 100; // Augmentation des tentatives pour la condition de distance
            bool foundValidPosition = false;

            if (ennemySpawnPoint == null)
            {
                // Vérification de sécurité: Si la distance minimale est trop grande pour la carte.
                // Cela empêche la boucle de 100 tentatives d'échouer systématiquement.
                if (mapRange < roundConfigurationSO.MinEnemySpawnDistanceFromPlayer)
                {
                    Debug.LogWarning("MinEnemySpawnDistanceFromPlayer is too large for the current map size. Reducing the required distance for spawn.");
                    // Utilise une distance maximale plausible (ex: 80% du rayon de la carte)
                    roundConfigurationSO.MinEnemySpawnDistanceFromPlayer = mapRange * 0.8f;
                }

                for (int i = 0; i < maxAttempts; i++)
                {
                    // Tente de trouver un point NavMesh aléatoire sur toute la zone
                    if (RandomPoint(waterSurface.transform.position, mapRange, out ennemySpawnPosition))
                    {
                        // Vérifie la distance configurable par rapport au joueur
                        if (Vector3.Distance(ennemySpawnPosition, playerController.transform.position) >= roundConfigurationSO.MinEnemySpawnDistanceFromPlayer)
                        {
                            foundValidPosition = true;
                            break;
                        }
                    }
                }

                // --- Fallback en cas d'échec de distance (après 100 tentatives) ---
                if (!foundValidPosition)
                {
                    Debug.LogWarning($"Fallback: Could not find distant enemy spawn after {maxAttempts} attempts. The enemy will be spawned closer to the center.");

                    // Dernier tirage valide sur la NavMesh sans la condition de distance
                    if (RandomPoint(waterSurface.transform.position, mapRange, out ennemySpawnPosition))
                    {
                        // Position valide sur NavMesh, mais potentiellement trop proche.
                    }
                    else
                    {
                        // Vraie erreur : la NavMesh n'existe pas.
                        Debug.LogError("FATAL ERROR: NavMesh position could not be found anywhere! Falling back to Vector3.zero.");
                        ennemySpawnPosition = Vector3.zero;
                    }
                }
            }
            else
            {
                ennemySpawnPosition = ennemySpawnPoint.position;
            }

            Debug.Log($"enemySpawnPoisition: {ennemySpawnPosition}");
            GameObject enemySubmarine = Instantiate(gameParametersSO.PlayersParameters.EnemyParameters.SubmarineData.Prefab, ennemySpawnPosition, Quaternion.identity) as GameObject;
            enemySubmarine.name = "EnemySubmarine" + index;
            enemySubmarine.TryGetComponent<SubmarineController>(out SubmarineController submarineController);

            // Submarine controller
            if (submarineController != null)
            {
                Debug.Log("RoundManager: initializing enemy submarinecontroller");
                submarineController.Init(gameParametersSO.PlayersParameters.EnemyParameters.SubmarineData);
            }

            // Global sonar context
            enemySubmarine.TryGetComponent<GlobalSonarContext>(out GlobalSonarContext globalSonarContext);
            if (globalSonarContext != null)
            {
                globalSonarContext.Init(gameParametersSO.PlayersParameters.EnemyParameters.GlobalSonarContext);
            }

            // Health controller
            enemySubmarine.TryGetComponent<HealthComponent>(out HealthComponent healthController);
            if (healthController != null)
            {
                healthController.Init(gameParametersSO.PlayersParameters.EnemyParameters.HealthCharacteristics);
            }

            // Equipments controller
            enemySubmarine.TryGetComponent<EquipmentController>(out EquipmentController equipmentController);
            if (equipmentController != null)
            {
                equipmentController.Init(gameParametersSO.PlayersParameters.EnemyParameters.StartEquipments);
            }

            // Weapons controller
            enemySubmarine.TryGetComponent<WeaponsController>(out WeaponsController weaponsController);
            if (weaponsController != null)
            {
                weaponsController.Init(gameParametersSO.PlayersParameters.EnemyParameters.StartWeapons);
                weaponsController.PlayerController = this.playerController;
            }

            AIEnemyContext aiContext = enemySubmarine.GetComponent<AIEnemyContext>();
            if (aiContext != null)
            {
                for (int i = 0; i < 15; i++)
                {
                    // Utilisation du RandomPoint dynamique pour les waypoints
                    if (RandomPoint(waterSurface.transform.position, mapRange, out Vector3 wpPoint))
                    {
                        GameObject waypoint = Instantiate(waypointPrefab, wpPoint, Quaternion.identity) as GameObject;
                        if (waypoint != null)
                        {
                            aiContext.AddWaypoint(waypoint.transform);
                        }
                    }
                }
            }

            playerInstances.Add(enemySubmarine);
        }

        private void HandleTimeoutEvent()
        {
            Debug.Log("Time is out.");
            messageEndEvent.OnEventRaised += HandleMessageEndEventVariant2;
            gameMessageEvent?.RaiseEvent(new GameMessage("Time out...", true, 5.0f));
            CleanRoundArea();
        }

        private void HandleDieEvent(GameObject deadObject)
        {
            Debug.Log("Die event");
            if (deadObject == null)
            {
                Debug.Log("Dead object is null.");
                return;
            }
            deadObject.TryGetComponent<PlayerController>(out PlayerController playerController);
            playerInstances.Remove(deadObject);
            if (playerController != null)
            {
                Destroy(deadObject);
                messageEndEvent.OnEventRaised += HandleMessageEndEventVariant2;
                gameMessageEvent?.RaiseEvent(new GameMessage("You are dead...", false, 5.0f));
                scoreEvent?.RaiseEvent(gameDataSO.EnemyData);
                StartCoroutine(DieRoutine());
            }
            else
            {
                Destroy(deadObject);
                scoreEvent?.RaiseEvent(gameDataSO.PlayerData);
                if (playerInstances.Count == 1)
                {
                    messageEndEvent.OnEventRaised += HandleMessageEndEventVariant2;
                    gameMessageEvent?.RaiseEvent(new GameMessage("You won!", false, 5.0f));
                    StartCoroutine(DieRoutine());
                }
                else
                {
                    gameMessageEvent?.RaiseEvent(new GameMessage("Enemy is dead!", false, 5.0f));
                    if (gameDataSO.RemainingEnemies.Value > 0)
                    {
                        gameDataSO.RemainingEnemies.Value--;
                    }
                }
            }
        }

        private IEnumerator DieRoutine()
        {
            yield return new WaitForSeconds(3f);
            CleanRoundArea();
        }

        // Dans RoundManager
        private void HandleMessageEndEventVariant2()
        {
            // Important: Se désabonner immédiatement au cas où d'autres messages sont dans la file
            messageEndEvent.OnEventRaised -= HandleMessageEndEventVariant2; 
            
            // **CHECK DE SÉCURITÉ**
            if (isScoreDisplayPending)
            {
                Debug.LogWarning("Score display sequence is already running. Ignoring redundant call.");
                return;
            }
            
            isScoreDisplayPending = true; // Empêche les appels ultérieurs immédiats
            
            UpdateScores();
            displayScoreEvent?.RaiseEvent();
            
            // Note: Le drapeau doit être réinitialisé par le GameManager au début d'un NOUVEAU round.
        }
        private void UpdateScores()
        {
            foreach (PlayerScoreDataSO playerScoreData in gameDataSO.PlayersScores)
            {
                playerScoreData.Points.Value += playerScoreData.CurrentRoundPoints.Value;
                playerScoreData.Points.Value += playerScoreData.CurrentRoundTimeBonusPoints.Value;
            }
        }

        private void HandlePauseEvent(bool needPauseGame)
        {
            ToggleGamePause(needPauseGame);
        }

        private void ToggleGamePause(bool needPauseGame)
        {
            if (needPauseGame)
            {
                Debug.Log("Game is paused.");
                Time.timeScale = 0;
            }
            else
            {
                Debug.Log("Game is unpaused.");
                Time.timeScale = 1;
            }
        }

        // =======================================================
        // MÉTHODES DYNAMIQUES
        // =======================================================

        /// <summary>
        /// Calcule le rayon maximal de la zone de jeu en fonction des limites du Collider (ou Renderer) de l'objet de surface.
        /// </summary>
        /// <param name="areaObject">L'objet représentant la zone de jeu (waterSurface).</param>
        /// <returns>La plus grande des demi-dimensions (Extents) X ou Z, ou 10.0f par défaut.</returns>
        private float GetMapMaxRadius(GameObject areaObject)
        {
            if (areaObject == null)
            {
                Debug.LogError("waterSurface is null. Cannot calculate map bounds.");
                return 10.0f;
            }

            Collider collider = areaObject.GetComponent<Collider>();
            if (collider != null)
            {
                // Retourne la plus grande des demi-dimensions (Extents) X ou Z.
                Vector3 extents = collider.bounds.extents;
                float radius = Mathf.Max(extents.x, extents.z);
                return radius;
            }

            Renderer renderer = areaObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                Vector3 extents = renderer.bounds.extents;
                return Mathf.Max(extents.x, extents.z);
            }

            Debug.LogWarning("waterSurface object does not have a Collider or Renderer. Using default radius 10.0f.");
            return 10.0f; // Valeur de repli
        }

        /// <summary>
        /// Tente de trouver un point aléatoire sur la NavMesh dans le rayon donné.
        /// La distance de recherche de NavMesh.SamplePosition est augmentée pour la robustesse.
        /// </summary>
        private bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            float maxSearchDistance = range + 2.0f;
            // Doit être la moitié du diamètre de votre sous-marin pour garantir un dégagement suffisant
            float safetyClearanceRadius = 1.0f; // Ajustez cette valeur si vos sous-marins sont plus larges

            for (int i = 0; i < 30; i++)
            {
                // Utilise toujours le tirage sur la zone complète
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;

                // 1. Échantillonnage NavMesh
                if (NavMesh.SamplePosition(randomPoint, out hit, maxSearchDistance, NavMesh.AllAreas))
                {
                    // 2. Vérification de la Distance au Bord (Cruciale avec de grands obstacles)
                    NavMeshHit edgeHit;
                    // On vérifie la distance du point trouvé à l'arête (bord) de la NavMesh.
                    if (NavMesh.FindClosestEdge(hit.position, out edgeHit, NavMesh.AllAreas))
                    {
                        // Si la distance au bord est inférieure au rayon de dégagement de l'entité, on est trop près de l'obstacle ou du bord de la carte.
                        if (edgeHit.distance < safetyClearanceRadius)
                        {
                            // Rejeter le point et passer à la tentative suivante
                            continue;
                        }
                    }

                    // Le point est sur la NavMesh ET a assez d'espace autour de lui.
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }
    }
}