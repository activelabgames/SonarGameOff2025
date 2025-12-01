using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "SubmarineData", menuName = "Sonar/Submarines/Submarine Data")]
    public class SubmarineDataSO : ScriptableObject
    {
        [SerializeField] public float BaseSpeed;
        [SerializeField] public bool CanMove = true;
        [SerializeField] public AudioClip ExplosionAudioClip;
        [SerializeField] public float ExplosionAudioClipVolume;

        [SerializeField] public GameObject Prefab;
    }    
}
