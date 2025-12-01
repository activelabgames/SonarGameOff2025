using UnityEngine;
using UnityEngine.Events;

public class EchoComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public float Lifetime;
    [SerializeField] public bool UnlimitedLifetime;
    
    [Header("Data")]
    [SerializeField] public GameObject DetectedObject;

    [Header("Events")]
    [SerializeField] GameObjectEventChannelSO representationDestroyedEvent;

    // Visualisation pour le debug dans l'inspecteur
    [SerializeField] private float lifeTimer = 0.0f; 

    private void OnEnable()
    {
        // On s'assure que le timer est à 0 quand l'objet est activé (pooling safe)
        lifeTimer = 0.0f;
    }

    private void Update()
    {
        if (UnlimitedLifetime) return;

        lifeTimer += Time.deltaTime;

        if (lifeTimer >= Lifetime)
        {
            // Sécurité pour éviter d'envoyer l'événement plusieurs fois
            lifeTimer = 0.0f; 
            representationDestroyedEvent?.RaiseEvent(gameObject);
        }
    }

    /// <summary>
    /// Appelé par le GlobalSonarContext quand une nouvelle info arrive pour cet écho.
    /// Cela empêche l'écho de disparaître tant qu'il est détecté.
    /// </summary>
    public void Refresh()
    {
        lifeTimer = 0.0f;
        // Ici, tu pourrais aussi remettre l'alpha à 100% si tu gères de la transparence
    }
}