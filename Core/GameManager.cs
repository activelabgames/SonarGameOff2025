using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.SearchService;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Sonar
{
    public class GameManager : MonoBehaviour
    {
        //[SerializeField] private PlayersDataSO playersData;
        //[SerializeField] private ScoresDataSO scoresData;
        [SerializeField] private InputReaderSO inputReader;
        [SerializeField] private Camera transitionCamera;
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private GameParametersSO gameParametersSO;

        [SerializeField] private GameObjectVariableSO playerSubmarinePrefabTD;
        [SerializeField] private GameObjectVariableSO enemySubmarinePrefabTD;
        [SerializeField] private WeaponsSetSO playerCurrentWeaponsListTD;
        [SerializeField] private WeaponsSetSO enemyCurrentWeaponsListTD;

        [SerializeField] private RoundConfigurationSO roundConfigurationSO;
        [SerializeField] private GameDataSO gameDataSO;

        // Main Menu Events
        [SerializeField] private EmptyEventChannelSO newGameEvent;
        [SerializeField] private EmptyEventChannelSO startCommandsEvent;
        [SerializeField] private EmptyEventChannelSO backFromCommandsGameEvent;
        // Round Events
        [SerializeField] private EmptyEventChannelSO roundStartedEvent;
        [SerializeField] private EmptyEventChannelSO roundEndedEvent;
        [SerializeField] EmptyEventChannelSO displayScoreEvent;
        

        [SerializeField] private AudioClipInformationEventChannelSO audioClipInformationEvent;
        [SerializeField] EmptyEventChannelSO hideScoreEvent;

        private void OnEnable()
        {
            newGameEvent.OnEventRaised += HandleNewGameEvent;
            startCommandsEvent.OnEventRaised += HandleCommandsEvent;
            backFromCommandsGameEvent.OnEventRaised += BackFromCommands;
            roundStartedEvent.OnEventRaised += StartRound;
            displayScoreEvent.OnEventRaised += HandleDisplayRoundEvent;
            roundEndedEvent.OnEventRaised += HandleRoundEndedEvent;
        }

        private void OnDestroy()
        {
            newGameEvent.OnEventRaised -= HandleNewGameEvent;
            startCommandsEvent.OnEventRaised -= HandleCommandsEvent;
            backFromCommandsGameEvent.OnEventRaised -= BackFromCommands;
            roundStartedEvent.OnEventRaised -= StartRound;
            displayScoreEvent.OnEventRaised -= HandleDisplayRoundEvent;
            roundEndedEvent.OnEventRaised -= HandleRoundEndedEvent;
        }

        private void Awake()
        {
            if (gameDataSO == null)
            {
                Debug.LogError("GameManager: GameDataSO is not assigned!");
            }
            transitionCamera.gameObject.SetActive(true);
        }

        private void Start()
        {
            //RoundConfigurationSO roundConfigurationSO = ScriptableObject.CreateInstance<RoundConfigurationSO>();

            DisplayMainMenu();
        }

        private void DisplayMainMenu()
        {
            Debug.Log("Displaying Main Menu...");
            // Implémentation de l'affichage du menu principal
            audioClipInformationEvent?.RaiseEvent(new AudioClipInformation(gameParametersSO.MainMenuParameters.AmbientMusic, gameParametersSO.MainMenuParameters.AmbientMusicVolume));
            StartCoroutine(LoadSceneCoroutine("MainMenuScene", LoadSceneMode.Additive));
        }

        private void HandleDisplayRoundEvent()
        {
            EndRound();
            audioClipInformationEvent?.RaiseEvent(new AudioClipInformation(null, 0.0f));
            StartCoroutine(LoadSceneCoroutine("ScorePanelScene", LoadSceneMode.Additive));
        }

        private void StartRound()
        {
            Debug.Log("Round " + (gameDataSO.CurrentRoundIndex.Value) + " started.");
            // Initialisation du round
            InitializeRoundScores();
            InitializeRoundGameData();
            audioClipInformationEvent?.RaiseEvent(new AudioClipInformation(gameParametersSO.RoundParameters[gameDataSO.CurrentRoundIndex.Value - 1].MainAmbience, gameParametersSO.RoundParameters[gameDataSO.CurrentRoundIndex.Value - 1].MainAmbienceVolume));
            StartCoroutine(LoadSceneCoroutine("RoundScene", LoadSceneMode.Additive));
            
        }

        private void DisplayCommands()
        {
            StartCoroutine(LoadSceneCoroutine("CommandsScene", LoadSceneMode.Additive));   
        }

        private void BackFromCommands()
        {
            SceneManager.UnloadSceneAsync("CommandsScene");
            DisplayMainMenu();
        }

        private IEnumerator LoadSceneCoroutine(string sceneName, LoadSceneMode loadSceneMode)
        {
            transitionCamera.gameObject.SetActive(true);
            //yield return new WaitForSeconds(1f); // Delay of 1 second (needed to display this scene).
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

            while (!asyncOperation.isDone)
            {
                Debug.Log($"Loading scene {sceneName}: {asyncOperation.progress * 100}%");
                yield return 0;
            }
            Debug.Log($"Scene {sceneName} loaded successfully.");
            transitionCamera.gameObject.SetActive(false);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }

        private IEnumerator UnloadSceneCoroutine(string sceneName)
        {
            //yield return new WaitForSeconds(1f); // Delay of 1 second (needed to display this scene).
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);

            while (!asyncOperation.isDone)
            {
                Debug.Log($"Unloading scene {sceneName}: {asyncOperation.progress * 100}%");
                yield return 0;
            }
            Debug.Log($"Scene {sceneName} unloaded successfully.");
        }

        private void HandleNewGameEvent()
        {
            Debug.Log("New Game event received.");
            gameDataSO.CurrentRoundIndex.Value = 1;
            InitializePlayersConfiguration();
            InitializeScores();
            StartCoroutine(UnloadSceneCoroutine("MainMenuScene"));
            StartRound();
        }

        private void HandleCommandsEvent()
        {
            Debug.Log("Commands event received.");
            StartCoroutine(UnloadSceneCoroutine("MainMenuScene"));
            DisplayCommands();
        }

        private void InitializePlayersConfiguration()
        {
            gameDataSO.PlayerData.PlayerName.Value = "Player";
            gameDataSO.PlayerData.PlayerScore.Player = gameDataSO.PlayerData;
            gameDataSO.EnemyData.PlayerName.Value = "Enemy";
        }
        private void InitializeRoundGameData()
        {
            if (roundConfigurationSO != null)
            {
                // TODO: move this to GameManager
                roundConfigurationSO.RoundDuration.Value = gameParametersSO.RoundParameters[gameDataSO.CurrentRoundIndex.Value - 1].RoundDuration;
                roundConfigurationSO.UnlimitedTime = gameParametersSO.RoundParameters[gameDataSO.CurrentRoundIndex.Value - 1].UnlimitedTime;
                roundConfigurationSO.EnemiesNumber = gameParametersSO.RoundParameters[gameDataSO.CurrentRoundIndex.Value - 1].EnemiesNumber;
                roundConfigurationSO.GameParameters = gameParametersSO;
            }
            //enemySubmarinePrefabTD.Value = gameParametersSO.PlayersParameters.EnemyParameters.SubmarinePrefab;
            //enemyCurrentWeaponsListTD.PrimaryWeaponAmmo.Value = enemyStartWeaponsList.PrimaryWeaponAmmo.Value;
            enemyCurrentWeaponsListTD.PrimaryWeaponAmmo.Value = gameParametersSO.PlayersParameters.EnemyParameters.StartWeapons.PrimaryWeaponAmmo.Value;
            //playerCurrentWeaponsListTD.PrimaryWeaponAmmo.Value = playerStartWeaponsList.PrimaryWeaponAmmo.Value;
            playerCurrentWeaponsListTD.PrimaryWeaponAmmo.Value = gameParametersSO.PlayersParameters.PlayerParameters.StartWeapons.PrimaryWeaponAmmo.Value;
        }

        private void InitializeScores()
        {
            foreach (PlayerScoreDataSO playerScoreData in gameDataSO.PlayersScores)
            {
                playerScoreData.Points.Value = 0;
                playerScoreData.CurrentRoundPoints.Value = 0;
                playerScoreData.CurrentRoundTimeBonusPoints.Value = 0;
            }
        }

        private void InitializeRoundScores()
        {
            foreach (PlayerScoreDataSO playerScoreData in gameDataSO.PlayersScores)
            {
                playerScoreData.CurrentRoundPoints.Value = 0;
                foreach (BaseScoreParameterSO scoreParameterSO in gameParametersSO.ScoreParameters.scoreParameters)
                {
                    MalusScoreParameterSO malusScoreParameterSO = scoreParameterSO as MalusScoreParameterSO;
                    if (malusScoreParameterSO == null)
                        continue;
                    playerScoreData.CurrentRoundTimeBonusPoints.Value += malusScoreParameterSO.InitialPointsToGive;
                }
            }
        }

        private void HandleRoundStartedEvent()
        {
            Debug.Log("Round started event received.");
            // Actions à effectuer lorsque le round commence
            StartRound();
        }

        private void HandleRoundEndedEvent()
        {
            SceneManager.UnloadSceneAsync("ScorePanelScene");
            gameDataSO.CurrentRoundIndex.Value++;
            if (gameDataSO.CurrentRoundIndex.Value <= gameParametersSO.RoundParameters.Count)
            {
                Debug.Log("GameManager: Loading next round.");
                StartRound();
            }
            else
            {
                Debug.Log("GameManager: It was the last round.");
                EndGame();
            }
        }

        private void EndRound()
        {
            Debug.Log("Round " + (gameDataSO.CurrentRoundIndex.Value) + " ended.");
            SceneManager.UnloadSceneAsync("RoundScene");
            // Nettoyage après le round
        }

        private void EndGame()
        {
            Debug.Log("Game ended.");
            // Afficher les scores finaux, statistiques, etc.
            transitionCamera.gameObject.SetActive(true);
            //SceneManager.UnloadSceneAsync("RoundScene");
            SceneManager.LoadSceneAsync("MainMenuScene", LoadSceneMode.Additive);
        }
    }
}
