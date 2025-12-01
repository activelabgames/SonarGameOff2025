using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    [Header("Tangage (Rotation)")]
    [Tooltip("Active ou désactive la rotation aléatoire")]
    public bool enableRotation = true;
    public float rotationStrength = 5.0f; // Degrés maximum
    public float rotationSpeed = 0.3f;

    // Point d'origine (Ancre)
    private Quaternion startRotation;

    // Graines aléatoires pour que X, Y et Z ne bougent pas de manière synchrone
    private Vector3 noiseOffsetPos;
    private Vector3 noiseOffsetRot;

    void Start()
    {
        startRotation = transform.rotation;

        // On génère des points de départ aléatoires dans le bruit de Perlin
        // pour que chaque axe ait son propre rythme.
        noiseOffsetPos = new Vector3(Random.value * 100f, Random.value * 100f, Random.value * 100f);
        noiseOffsetRot = new Vector3(Random.value * 100f, Random.value * 100f, Random.value * 100f);
    }

    void Update()
    {   
        if (enableRotation)
        {
            ApplyRotationFloat();
        }
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
}