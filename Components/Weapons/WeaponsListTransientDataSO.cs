using UnityEngine;
using System.Collections.Generic;

namespace Sonar
{
    [CreateAssetMenu(fileName = "WeaponsList", menuName = "Sonar/Transient/Weapons/Weapons List")]
    public class WeaponsListTransientDataSO : ScriptableObject
    {
        public BaseWeaponDataSO PrimaryWeapon;
        public int PrimaryWeaponCurrentAmmo;
        public BaseWeaponDataSO SecondaryWeapon;
        public int SecondaryWeaponCurrentAmmo;
    }    
}
