using Unity.VisualScripting;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sonar
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] UIDocument uiDocument;
        [SerializeField] UIDocument fadeOverlayUIDocument;
        private VisualElement root;
        private VisualElement fadeOverlayRoot;
        [SerializeField] private EmptyEventChannelSO startGameEvent;
        [SerializeField] private EmptyEventChannelSO startCommandsEvent;

        // Used to change the SoundManager position to diegetic game menu object
        [SerializeField] private Vector3EventChannelSO MenuPositionEvent;

        public float transitionDuration = 1.5f;

        [Header("Audio & FX")]
        public AudioSource audioSource;
        public AudioClip hoverSound;
        public AudioClip clickSound;
        public ParticleSystem clickParticlePrefab;

        private VisualElement fadeOverlay;
        private bool isTransitioning = false;

        private void OnEnable()
        {

            fadeOverlayRoot = fadeOverlayUIDocument.rootVisualElement;
            fadeOverlay = fadeOverlayRoot.Q<VisualElement>("FadeOverlay");
            if (fadeOverlay != null)
            {
                fadeOverlay.style.opacity = 0.0f;
                fadeOverlay.pickingMode = PickingMode.Ignore;
            }
            
            
            
            root = uiDocument.rootVisualElement;

            Button startGameButton = root.Q<Button>("NewGameButton");
            Debug.Log($"MainMenuController: Subscribing to Start Game button click event: {startGameButton}");
            startGameButton.RegisterCallback<ClickEvent>(evt => OnStartGameButtonPressed());
            startGameButton.RegisterCallback<MouseEnterEvent>(OnStartGameButtonHovered);

            Button commandsButton = root.Q<Button>("CommandsButton");
            Debug.Log($"MainMenuController: Subscribing to Commands Game button click event: {commandsButton}");
            commandsButton.RegisterCallback<ClickEvent>(evt => OnCommandsButtonPressed());
            commandsButton.RegisterCallback<MouseEnterEvent>(OnCommandsButtonHovered);

            Button exitButton = root.Q<Button>("ExitButton");
            exitButton.RegisterCallback<ClickEvent>(evt => Application.Quit());

            MenuPositionEvent?.RaiseEvent(transform.position);
        }
        private void OnDisable()
        {
            Button startGameButton = root.Q<Button>("NewGameButton");
            startGameButton.clicked -= OnStartGameButtonPressed;
        }

        private void OnStartGameButtonHovered(MouseEnterEvent evt)
        {
            if (audioSource != null && hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound, .5f);
            }
        }

        private void OnCommandsButtonHovered(MouseEnterEvent evt)
        {
            if (audioSource != null && hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound, .5f);
            }
        }

        public void OnStartGameButtonPressed()
        {
            Debug.Log("MainMenuController: Start Game button pressed.");
            if (isTransitioning) return;
            if (audioSource != null && clickSound != null)
            {
                audioSource.PlayOneShot(clickSound, .5f);
            }
            StartCoroutine(LoadGameScene());
        }

        public IEnumerator LoadGameScene()
        {
            isTransitioning = true;
            Debug.Log("MainMenuController: Loading Game Scene.");
            Debug.Log($"MainMenuController: FadeOverlay:{fadeOverlay}");
            if (fadeOverlay != null)
            {
                float timer = 0f;
                while (timer < transitionDuration)
                {
                    timer += Time.deltaTime;
                    Debug.Log($"Timer");
                    Debug.Log($"FadeOverlay opacity: {fadeOverlay.style.opacity}");
                    fadeOverlay.style.opacity = Mathf.Lerp(0f, 1f, timer / transitionDuration);
                    yield return null;
                }
                fadeOverlay.style.opacity = 1.0f;
            }
            yield return new WaitForSeconds(0.5f);
            startGameEvent.RaiseEvent();
        }


        public void OnCommandsButtonPressed()
        {
            Debug.Log("MainMenuController: Start Game button pressed.");
            if (isTransitioning) return;
            if (audioSource != null && clickSound != null)
            {
                audioSource.PlayOneShot(clickSound, .5f);
            }
            StartCoroutine(LoadCommandsScene());
        }

        public IEnumerator LoadCommandsScene()
        {
            isTransitioning = true;
            Debug.Log("MainMenuController: Loading Commands Scene.");
            Debug.Log($"MainMenuController: FadeOverlay:{fadeOverlay}");
            if (fadeOverlay != null)
            {
                float timer = 0f;
                while (timer < transitionDuration)
                {
                    timer += Time.deltaTime;
                    Debug.Log($"Timer");
                    Debug.Log($"FadeOverlay opacity: {fadeOverlay.style.opacity}");
                    fadeOverlay.style.opacity = Mathf.Lerp(0f, 1f, timer / transitionDuration);
                    yield return null;
                }
                fadeOverlay.style.opacity = 1.0f;
            }
            yield return new WaitForSeconds(0.5f);
            startCommandsEvent.RaiseEvent();
        }

        private void SpawnParticleEffect(Vector2 position)
        {
            if (clickParticlePrefab == null) return;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 5f;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            ParticleSystem fx = Instantiate(clickParticlePrefab, worldPos, Quaternion.identity);
            Destroy(fx.gameObject, 2.0f);
        }
    }
}