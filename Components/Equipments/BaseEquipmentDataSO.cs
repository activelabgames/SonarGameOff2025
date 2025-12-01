using UnityEngine;

namespace Sonar
{
    public abstract class BaseEquipmentDataSO : ScriptableObject
    {
        public string EquipmentName;
        public string EquipmentDescription;
        public GameObject EquipmentPrefab;
        public Sprite EquipmentIcon;
        //public float weight;
        public bool IsPassive;

        public bool IsMute;

        private void OnEnable()
        {
            Enable();
        }
        private void OnDisable()
        {
            Disable();
        }

        public abstract void Enable();

        public abstract void Disable();

        public virtual void Behave(EquipmentController equipmentController)
        {
            //Debug.Log($"Equipment '{EquipmentName}' is used.");
        }
    }    
}
