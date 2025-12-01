using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "GlobalSonarContext", menuName = "Sonar/Configuration/Global Sonar Context")]
    public class GlobalSonarContextSO : ScriptableObject
    {
        [SerializeField] public float EchoLifetime = 5.0f;
        [SerializeField] public float ActiveEchoPersistenceTime = 5.0f;
        [SerializeField] public float DetectionPauseTime = 1.0f;
        [SerializeField] public SonarPolicySO SonarPolicy;
        [SerializeField] public Color minimumEchoSphereGizmoColor = new Color(0f, 0.3f, .5f, 0.4f);
        [SerializeField] public Color minimumEchoSphereGizmoContourColor = new Color(0f, 0.3f, .5f, 1f);

        [SerializeField] public Color minimumIdentificationSphereGizmoColor = new Color(0f, 0.3f, .2f, 0.4f);
        [SerializeField] public Color minimumIdentificationSphereGizmoContourColor = new Color(0f, 0.3f, .2f, 1f);

        [SerializeField] public AudioClip AudioClip;
        [SerializeField] public float AudioClipVolume = 1.0f;
        public bool IsMute;
    }    
}
