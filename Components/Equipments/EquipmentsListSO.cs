using UnityEngine;
using System.Collections.Generic;

namespace Sonar
{
    [CreateAssetMenu(fileName = "EquipmentsList", menuName = "Sonar/Equipments/Equipments List")]
    public class EquipmentsListSO : ScriptableObject
    {
        public PassiveSonarDataSO PassiveSonar;
        public ActiveSonarDataSO ActiveSonar;
        public SimpleEngineDataSO SimpleEngine;
    }    
}
