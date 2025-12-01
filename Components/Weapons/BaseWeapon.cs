// BaseWeapon.cs

using System.Runtime.CompilerServices;
using UnityEngine;
using Sonar.AI; // 🚨 AJOUT : Pour IWeaponLimiter

namespace Sonar
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        [SerializeField] protected BaseWeaponDataSO weaponData;
        [SerializeField] protected WeaponsController weaponsController;

        protected Vector3 targetPosition;
        protected bool isInitialized = false;

        // 🚀 AJOUT : Référence au limiteur d'armes (l'AIEnemyContext)
        private IWeaponLimiter _limiter;

        // Méthode Init existante (pour le joueur)
        public virtual void Init(WeaponsController weaponsController, Vector3 targetPosition)
        {
            if (isInitialized)
            {
                Debug.Log("Trying to call Init on an already initialized weapon. Aborting.");
                return;
            }
            this.weaponsController = weaponsController;
            this.targetPosition = targetPosition;
            isInitialized = true;
        }
        
        // 🚀 NOUVELLE SURCHARGE Init (pour les tirs de l'IA)
        public virtual void Init(WeaponsController weaponsController, Vector3 targetPosition, IWeaponLimiter limiter)
        {
            // Appeler l'initialisation de base
            Init(weaponsController, targetPosition);
            
            if (limiter == null) return;
            
            this._limiter = limiter;
            
            // 🚨 INCLEMENTATION DU COMPTEUR DE L'IA LORS DE L'INSTANCIATION
            _limiter.CurrentActiveTorpedos++;
        }

        public virtual void Behave(WeaponsController weaponsController)
        {
            weaponData.Behave(weaponsController);
        }
        
        // 🚀 AJOUT : Décrémentation du compteur lors de la destruction
        protected virtual void OnDestroy()
        {
            // Si la torpille a été initialisée avec un limiteur d'IA
            if (_limiter != null)
            {
                // 🚨 DÉCREMENTATION DU COMPTEUR DE L'IA
                _limiter.CurrentActiveTorpedos--;
            }
        }
    }    
}