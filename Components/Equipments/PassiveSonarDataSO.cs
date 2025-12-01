using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "PassiveSonarData", menuName = "Sonar/Equipments/Passive Sonar")]
    public class PassiveSonarDataSO : BaseSonarDataSO
    {

        public override string ToString()
        {
            return $"{EquipmentName}: {EquipmentDescription} (Range: {DetectionRange})";
        }

        public override void Enable()
        {
            Debug.Log($"Passive Sonar '{EquipmentName}' enabled.");
        }

        public override void Disable()
        {
            Debug.Log($"Passive Sonar '{EquipmentName}' disabled.");
        }

        public override void Behave(EquipmentController equipmentController)
        {
            /*
            // Logic for passive sonar behavior, e.g., continuously scanning for objects
            Debug.Log($"Passive Sonar '{EquipmentName}' is scanning for objects within {DetectionRange} units.");
            // Example: Use Physics.OverlapSphere to find nearby objects
            Collider[] hits = Physics.OverlapSphere(equipmentController.transform.position, DetectionRange, DetectionMask);

            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    EquipmentController targetequipment = hit.GetComponent<EquipmentController>();
                    if (targetequipment != null && targetequipment != equipmentController)
                    {
                        Debug.Log($"Passive Sonar '{EquipmentName}' detected equipment: {targetequipment.EquipmentData.EquipmentName}");
                    }
                }
            }
            else
            {
                Debug.Log($"Passive Sonar '{EquipmentName}' detected no objects.");
            }*/
        }

    }    
}
