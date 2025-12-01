using UnityEngine;
using System.Collections.Generic;
using Sonar;

[CreateAssetMenu(fileName = "WeaponsParametersContainer", menuName = "Sonar/Parameters/Weapons Parameters Container")]
public class WeaponsParametersContainerSO : ScriptableObject
{
    public List<BaseWeaponDataSO> WeaponsParameters;
}