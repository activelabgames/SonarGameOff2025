using Unity.VisualScripting;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sonar
{
    public class ScorePanelController : MonoBehaviour
    {
        [SerializeField] UIDocument uiDocument;
        [SerializeField] UIDocument fadeOverlayUIDocument;
        private VisualElement root;
        private VisualElement fadeOverlayRoot;

        [SerializeField] private EmptyEventChannelSO roundEndedGameEvent;

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

            Button continueButton = root.Q<Button>("ContinueButton");
            Debug.Log($"ScorePanelController: Subscribing to Continue button click event: {continueButton}");
            continueButton.RegisterCallback<ClickEvent>(evt => OnContinueButtonPressed());
            continueButton.RegisterCallback<MouseEnterEvent>(OnContinueButtonHovered);

            /*Button exitButton = root.Q<Button>("ExitButton");
            exitButton.RegisterCallback<ClickEvent>(evt => Application.Quit());*/

            MenuPositionEvent?.RaiseEvent(transform.position);
        }
        private void OnDisable()
        {
            Button continueButton = root.Q<Button>("ContinueButton");
            continueButton.clicked -= OnContinueButtonPressed;
        }

        private void OnContinueButtonHovered(MouseEnterEvent evt)
        {
            if (audioSource != null && hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound, .5f);
            }
        }

        public void OnContinueButtonPressed()
        {
            Debug.Log("ScorePanelController: Continue button pressed.");
            if (isTransitioning) return;
            if (audioSource != null && clickSound != null)
            {
                audioSource.PlayOneShot(clickSound, .5f);
            }
            StartCoroutine(LoadNextScene());
        }

        public IEnumerator LoadNextScene()
        {
            isTransitioning = true;
            Debug.Log("ScorePanelController: Loading Game Scene.");
            Debug.Log($"ScorePanelController: FadeOverlay:{fadeOverlay}");
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
            roundEndedGameEvent.RaiseEvent();
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