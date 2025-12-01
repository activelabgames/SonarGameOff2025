using Sonar;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParameters", menuName = "Sonar/Parameters/Player")]
public class PlayerParametersSO : BaseParametersSO
{
    public SubmarineDataSO SubmarineData;
    public HealthCharacteristicsSO HealthCharacteristics;
    public WeaponsSetSO StartWeapons;
    public EquipmentsListSO StartEquipments;

    public GlobalSonarContextSO GlobalSonarContext;
}