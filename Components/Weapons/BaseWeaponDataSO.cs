using UnityEngine;

namespace Sonar
{
    public abstract class BaseWeaponDataSO : ScriptableObject
    {
        public string WeaponName;
        public string WeaponDescription;
        public GameObject WeaponPrefab;
        public Sprite WeaponIcon;

        public float TimeToTrigger;
        //public float weight;

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

        public virtual void Behave(WeaponsController WeaponsController)
        {
            //Debug.Log($"Weapon '{WeaponName}' is used.");
        }
    }    
}
