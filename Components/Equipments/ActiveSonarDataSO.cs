using Unity.VisualScripting;
using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "ActiveSonarData", menuName = "Sonar/Equipments/Active Sonar")]
    public class ActiveSonarDataSO : BaseSonarDataSO
    {
        public float SonarWaveSpeed;
        public float PulseWidth;

        public bool VisualEffectEnabled;

        public float SoundIntensity;

        public AudioClip AudioClip;
        public float AudioClipVolume = 1.0f;

        [SerializeField] private DetectionInformationEventChannelSO identificationInformationEvent;

        

        public override string ToString()
        {
            return $"{EquipmentName}: {EquipmentDescription} (Detection Range: {DetectionRange}, CoolDown: {CoolDown})";
        }

        public override void Enable()
        {
            //Debug.Log($"Active Sonar '{EquipmentName}' enabled.");
            
        }

        public override void Disable()
        {
            //Debug.Log($"Active Sonar '{EquipmentName}' disabled.");
        }

        public override void Behave(EquipmentController equipmentController)
        {

        }


    }    
}
