using UnityEngine;
using System.Collections.Generic;

namespace Sonar
{
    [CreateAssetMenu(fileName = "WeaponsSet", menuName = "Sonar/Weapons/Weapons Set")]
    public class WeaponsSetSO : ScriptableObject
    {
        public BaseWeaponDataSO PrimaryWeapon;
        public IntVariableSO PrimaryWeaponAmmo;
        public bool UnlimitedPrimaryWeaponAmmos;
        public int MaxConcurrentPrimaryWeaponAllowed;
        public BaseWeaponDataSO SecondaryWeapon;
        public IntVariableSO SecondaryWeaponAmmo;
        public bool UnlimitedSecondaryWeaponAmmos;
        public int MaxConcurrentSecondaryWeaponAllowed;
    }    
}
