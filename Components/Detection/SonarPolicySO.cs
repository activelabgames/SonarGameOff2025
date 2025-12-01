using UnityEngine;

namespace Sonar
{

    [CreateAssetMenu(fileName = "SonarPolicy", menuName = "Sonar/Detection/Sonar Policy", order = 0)]
    public class SonarPolicySO : ScriptableObject {
        [SerializeField] public bool RegularlyScanning;
        [SerializeField] public float ScanFrequency;
        [SerializeField] public bool ScanOnUnknownEcho;
        [SerializeField] public bool ScanOnIdentifiedEcho;
    }    
}
