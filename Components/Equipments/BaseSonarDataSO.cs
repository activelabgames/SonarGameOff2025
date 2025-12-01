using Unity.VisualScripting;
using UnityEngine;

namespace Sonar
{
    public abstract class BaseSonarDataSO: BaseEquipmentDataSO
    {
        public float DetectionRange;
        public LayerMask DetectionMask;

        public float MaxEngineIntensityHandled = 100f;
        //public FloatVariableSO MaxEngineIntensityHandled;
        public float DecayExponent = 4f;
        //public FloatVariableSO DecayExponent;

        [Range(0f, 1f)]
        //public float DetectionRatio = 0.999f;
        public float DetectionRatio;

        [Range(0f, 1f)]
        //public float IdentificationRatio = 0.6f;
        public float IdentificationRatio;

        [SerializeField] public Color DetectionRangeSphereGizmoColor = new Color(0f, 0.7f, 1f, .4f);
        [SerializeField] public Color DetectionRangeSphereGizmoContourColor = new Color(0f, 0.7f, 1f, 1f);


        [SerializeField] public Color IdentificationRangeSphereGizmoColor = new Color(0.2f, 0.3f, 1f, .4f);
        [SerializeField] public Color IdentificationRangeSphereGizmoContourColor = new Color(0.2f, 0.3f, 1f, 1f);

        public bool IsGizmoEnabled = true;

        public float CoolDown;

        public override string ToString()
        {
            return $"{EquipmentName}: {EquipmentDescription} (Detection Range: {DetectionRange}, CoolDown: {CoolDown})";
        }

        public abstract override void Enable();

        public abstract override void Disable();

        public abstract override void Behave(EquipmentController equipmentController);
    }    
}
