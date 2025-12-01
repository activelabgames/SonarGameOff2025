using UnityEngine;
using System.Collections.Generic;
namespace Sonar
{
    [CreateAssetMenu(fileName = "AllData", menuName = "Sonar/Data/All Data Container")]
    public class AllDataContainerSO : ScriptableObject
    {
        public List<BaseDataSO> GameSystemsData;
    }    
}
