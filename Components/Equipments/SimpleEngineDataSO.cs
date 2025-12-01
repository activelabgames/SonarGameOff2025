using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "SimpleEngineData", menuName = "Sonar/Equipments/Simple Engine")]
    public class SimpleEngineDataSO : BaseEquipmentDataSO
    {
        public float SoundIntensity;

        public AudioClip AudioClip;
        public float AudioVolume;
        public bool IsStarted = true;

        public BoolVariableSO EngineEnabledVariable;

        public override string ToString()
        {
            return $"{EquipmentName}: {EquipmentDescription} (Sound Intensity: {SoundIntensity})";
        }

        public override void Enable()
        {
            Debug.Log($"Enabling Simple Engine '{EquipmentName}' with sound intensity {SoundIntensity}");
        }

        public override void Disable()
        {
            Debug.Log($"Disabling Simple Engine '{EquipmentName}'");
        }

        public override void Behave(EquipmentController equipmentController)
        {
            //Debug.Log($"Simple Engine '{EquipmentName}' is active with sound intensity {SoundIntensity}");
            // Implement engine-specific behavior here
        }
    }    
}
