using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    [Header("Mouvement (Position)")]
    [Tooltip("La distance maximum parcourue depuis le centre sur chaque axe")]
    public float moveRadius = 0.5f; 
    
    [Tooltip("La vitesse de la dérive (plus bas = plus lourd/aquatique)")]
    public float moveSpeed = 0.5f;

    [Header("Tangage (Rotation)")]
    [Tooltip("Active ou désactive la rotation aléatoire")]
    public bool enableRotation = true;
    public float rotationStrength = 5.0f; // Degrés maximum
    public float rotationSpeed = 0.3f;

    // Point d'origine (Ancre)
    private Vector3 startPosition;
    private Quaternion startRotation;

    // Graines aléatoires pour que X, Y et Z ne bougent pas de manière synchrone
    private Vector3 noiseOffsetPos;
    private Vector3 noiseOffsetRot;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        // On génère des points de départ aléatoires dans le bruit de Perlin
        // pour que chaque axe ait son propre rythme.
        noiseOffsetPos = new Vector3(Random.value * 100f, Random.value * 100f, Random.value * 100f);
        noiseOffsetRot = new Vector3(Random.value * 100f, Random.value * 100f, Random.value * 100f);
    }

    void Update()
    {
        ApplyPositionFloat();
        
        if (enableRotation)
        {
            ApplyRotationFloat();
        }
    }

    void ApplyPositionFloat()
    {
        // Mathf.PerlinNoise renvoie une valeur entre 0 et 1.
        // Le temps fait avancer le curseur sur la courbe de bruit.
        float time = Time.time * moveSpeed;

        // Calcul pour chaque axe
        float x = Mathf.PerlinNoise(time + noiseOffsetPos.x, 0);
        float y = Mathf.PerlinNoise(time + noiseOffsetPos.y, 1);
        float z = Mathf.PerlinNoise(time + noiseOffsetPos.z, 2);

        // On convertit la plage [0, 1] vers [-1, 1] pour aller dans les deux sens
        // Puis on multiplie par le rayon autorisé.
        Vector3 offset = new Vector3(
            (x * 2 - 1) * moveRadius,
            (y * 2 - 1) * moveRadius,
            (z * 2 - 1) * moveRadius
        );

        transform.position = startPosition + offset;
    }

    void ApplyRotationFloat()
    {
        float time = Time.time * rotationSpeed;

        // Même logique pour la rotation
        float rotX = (Mathf.PerlinNoise(time + noiseOffsetRot.x, 0) * 2 - 1) * rotationStrength;
        float rotY = (Mathf.PerlinNoise(time + noiseOffsetRot.y, 1) * 2 - 1) * rotationStrength;
        float rotZ = (Mathf.PerlinNoise(time + noiseOffsetRot.z, 2) * 2 - 1) * rotationStrength;

        // On applique la rotation par dessus la rotation initiale
        transform.rotation = startRotation * Quaternion.Euler(rotX, rotY, rotZ);
    }

    // Affiche une sphère filaire dans l'éditeur pour visualiser la zone de mouvement
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        // Si le jeu tourne, on montre la zone autour du point de départ, sinon autour de la position actuelle
        Vector3 center = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawWireCube(center, Vector3.one * moveRadius * 2);
    }
}