using UnityEngine;
using UnityEngine.UIElements;

namespace Sonar
{
    public class OldScorePanelController : MonoBehaviour
    {
        [SerializeField] UIDocument uiDocument;
        private VisualElement root;
        [SerializeField] private EmptyEventChannelSO roundEndedGameEvent;

        [SerializeField] private PlayerScoreDataSO playerScoreDataSO;
        [SerializeField] private PlayerScoreDataSO enemyScoreDataSO;

        private void OnEnable()
        {
            if (uiDocument == null)
            {
                Debug.LogError("ScorePanelController: UIDocument is not assigned!");
                return;
            }
            if (playerScoreDataSO == null)
            {
                Debug.LogError("ScorePanelController: PlayerScoreDataSO is not assigned!");
                return;
            }
            if (enemyScoreDataSO == null)
            {
                Debug.LogError("ScorePanelController: EnemyScoreDataSO is not assigned!");
                return;
            }
            root = uiDocument.rootVisualElement;
            Button continueButton = root.Q<Button>("ContinueButton");
            Debug.Log($"ScorePanelController: Subscribing to Continue button click event: {continueButton}");
            continueButton.RegisterCallback<ClickEvent>(evt => OnContinueButtonPressed());

            Label totalPlayerPointsLabel = root.Q<Label>("PlayerTotalLabel");
            Label totalEnemyPointsLabel = root.Q<Label>("EnemyTotalLabel");

            if (totalPlayerPointsLabel != null)
            {
                totalPlayerPointsLabel.bindingPath = nameof(playerScoreDataSO.CurrentRoundTotalPoints);
            }
            else
            {
                Debug.LogError("ScorePanelController: PlayerTotalLabel not found in UI.");
            }
            if (totalEnemyPointsLabel != null)
            {
                totalEnemyPointsLabel.bindingPath = nameof(enemyScoreDataSO.CurrentRoundTotalPoints);
            }
            else
            {
                Debug.LogError("ScorePanelController: EnemyTotalLabel not found in UI.");
            }
        }
        private void OnDisable()
        {
            Button continueButton = root.Q<Button>("ContinueButton");
            continueButton.clicked -= OnContinueButtonPressed;
        }

        public void OnContinueButtonPressed()
        {
            Debug.Log("ScorePanelController: Continue button pressed.");
            roundEndedGameEvent.RaiseEvent();
            gameObject.SetActive(false);
        }
    }
}