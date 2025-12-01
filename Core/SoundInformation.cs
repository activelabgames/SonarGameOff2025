using UnityEngine;

public class SoundInformation
{
    private GameObject source;
    public GameObject Source => source;
    private Vector3 location;
    public Vector3 Location => location;
    private SoundCategorySO category;
    public SoundCategorySO Category => category;
    private float intensity;
    public float Intensity => intensity;

    public SoundInformation(GameObject source, Vector3 location, SoundCategorySO category, float intensity)
    {
        this.source = source;
        this.location = location;
        this.category = category;
        this.intensity = intensity;
    }
}