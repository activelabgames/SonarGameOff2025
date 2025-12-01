using System.Runtime.CompilerServices;
using UnityEngine;

namespace Sonar
{
    public abstract class BaseEquipment : MonoBehaviour
    {
        public BaseEquipmentDataSO EquipmentData;
        public EquipmentController EquipmentController;

        [SerializeField] private bool isInitialized = false;
        public bool IsInitialized => isInitialized;

        public virtual void Init(EquipmentController theEquipmentController, BaseEquipmentDataSO baseEquipmentDataSO)
        {
            EquipmentController = theEquipmentController;
            EquipmentData = baseEquipmentDataSO;
            isInitialized = true;
        }

        public virtual void Behave(EquipmentController EquipmentController)
        {
            if (isInitialized)
            {
                EquipmentData.Behave(EquipmentController);   
            }
            else
            {
                Debug.Log($"Equipment {gameObject.name} was not initialized.");
            }
        }
    }    
}
