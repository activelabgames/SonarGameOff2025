using UnityEngine;
using System.Collections.Generic;
using Sonar;

[CreateAssetMenu(fileName = "EquipmentsParametersContainer", menuName = "Sonar/Parameters/Equipments Parameters Container")]
public class EquipmentsParametersContainerSO : ScriptableObject
{
    public List<BaseWeaponDataSO> EquipmentsParameters;
}