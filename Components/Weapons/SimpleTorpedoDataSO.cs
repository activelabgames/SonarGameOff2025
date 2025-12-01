using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "Simple Torpedo", menuName = "Sonar/Weapons/Simple Torpedo")]
    public class SimpleTorpedoDataSO : BaseTorpedoDataSO
    {
        public override void Enable()
        {
    //       Debug.Log($"Torpedo '{TorpedoName}' enabled.");
        }

        public override void Disable()
        {
    //        Debug.Log($"Torpedo '{TorpedoName}' disabled.");
        }
    }    
}
