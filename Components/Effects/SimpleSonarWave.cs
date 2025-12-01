using UnityEngine;

namespace Sonar
{
    public class SimpleSonarWave : MonoBehaviour
    {
        [SerializeField] private ActiveSonar activeSonar;

        // Référence au Material pour modifier sa couleur et sa transparence.
        private Material _waveMaterial;

        // Stocke la couleur de base pour pouvoir réinitialiser la transparence.
        private Color _initialColor;

        // Le drapeau pour indiquer si l'onde est en cours d'animation.
        private bool _isActive = false;

        private bool isInitialized = false;

        void Start()
        {
            // Récupère l'instance du Material du composant Renderer.
            // Cela permet de modifier cette instance sans affecter l'asset original.
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                _waveMaterial = renderer.material;
                _initialColor = _waveMaterial.color;
            }
            else
            {
                Debug.LogError("Le composant Renderer est manquant sur l'objet Sonar Wave.");
                enabled = false; // Désactive le script si le renderer n'est pas trouvé.
            }

            // Cache l'objet au démarrage.
            gameObject.SetActive(false);
        }

        void Update()
        {
            if (_isActive)
            {
                // 1. Agrandissement : augmente la taille du Quad
                transform.localScale += Vector3.one * activeSonar.ActiveSonarData.SonarWaveSpeed * Time.deltaTime;

                // 2. Calcul du Fade Out (Transparence) :
                // Calcule le ratio entre la taille actuelle et la taille max.
                float ratio = transform.localScale.x / activeSonar.ActiveSonarData.DetectionRange;

                // L'Alpha commence à 1.0 (opaque) et tend vers 0.0 (transparent) quand ratio = 1.0.
                float currentAlpha = Mathf.Clamp01(1.0f - ratio);

                // 3. Application de la transparence :
                Color newColor = _initialColor;
                newColor.a = currentAlpha;
                _waveMaterial.color = newColor;

                // 4. Fin de l'animation :
                if (transform.localScale.x >= activeSonar.ActiveSonarData.DetectionRange)
                {
                    // Réinitialise l'état pour la prochaine activation.
                    gameObject.SetActive(false);
                    _isActive = false;
                    transform.localScale = Vector3.zero; // Très important pour repartir de zéro.
                }
            }
        }

        /// <summary>
        /// Déclenche l'animation de l'onde de sonar.
        /// Cette fonction doit être appelée par le script PlayerSonarController.
        /// </summary>
        public void FireSonar()
        {
            if (_isActive) return; // Ignore l'appel si une onde est déjà en cours.

            // Initialisation :
            transform.localScale = Vector3.zero;

            // S'assure que l'onde est visible et à son opacité maximale au départ.
            _waveMaterial.color = _initialColor;

            // Activation :
            gameObject.SetActive(true);
            _isActive = true;
        }
    }    
}
