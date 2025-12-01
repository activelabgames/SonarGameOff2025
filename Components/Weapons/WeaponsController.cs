// WeaponsController.cs

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sonar.AI; // 🚨 AJOUT : Pour accéder à IWeaponLimiter

namespace Sonar
{
    public class WeaponsController : MonoBehaviour
    {
        [SerializeField] private WeaponsSetSO startWeapons;

        public LayerMask ClickableLayer;

        public List<GameObject> CurrentPrimaryWeaponInstancesActive = new List<GameObject>();
        public List<GameObject> CurrentSecondaryWeaponInstancesActive = new List<GameObject>();

        [SerializeField] public BaseWeaponDataSO primaryWeapon;
        [SerializeField] private int primaryWeaponAmmo;
        [SerializeField] private bool unlimitedPrimaryWeaponAmmos;
        [SerializeField] private int maxConcurrentPrimaryWeaponAllowed;
        [SerializeField] private BaseWeaponDataSO secondaryWeapon;
        [SerializeField] private int secondaryWeaponAmmo;
        [SerializeField] private bool unlimitedSecondaryWeaponAmmos;
        [SerializeField] private int maxConcurrentSecondaryWeaponAllowed;
        
        private BaseWeaponDataSO equippedWeapon;

        [SerializeField] private Transform torpedoLaunchPoint;

        [SerializeField] private Vector2EventChannelSO UseWeaponEvent;
        [SerializeField] private GameObjectAndVector3EventChannelSO UsePrimaryWeaponEvent;
        [SerializeField] private GameObjectAndVector3EventChannelSO UseSecondaryWeaponEvent;

        [SerializeField] private FloatEventChannelSO ammoEvent;

        [SerializeField] public PlayerController PlayerController;

        [SerializeField] private bool isInitialized = false;

        // 🚀 AJOUTS : Minuteries pour le temps de mise en œuvre
        private float nextPrimaryWeaponShotTime = 0f; 
        private float nextSecondaryWeaponShotTime = 0f; 
        

        private void OnEnable()
        {
            if(UseWeaponEvent != null)
            {
                UseWeaponEvent.OnEventRaised += HandleUseWeaponEvent;
            }
            UsePrimaryWeaponEvent.OnEventRaised += HandlePrimaryWeaponEvent;
            UseSecondaryWeaponEvent.OnEventRaised += HandleSecondaryWeaponEvent;
        }

        private void OnDisable()
        {
            if(UseWeaponEvent != null)
            {
                UseWeaponEvent.OnEventRaised -= HandleUseWeaponEvent;    
            }
            UsePrimaryWeaponEvent.OnEventRaised -= HandlePrimaryWeaponEvent;
            UseSecondaryWeaponEvent.OnEventRaised -= HandleSecondaryWeaponEvent;
        }

        public void Init(WeaponsSetSO startWeapons)
        {
            Debug.Log($"WeaponsController: event is null = {ammoEvent == null}");
            this.startWeapons = startWeapons;
            this.primaryWeapon = startWeapons.PrimaryWeapon;
            if (this.primaryWeapon != null)
            {
                this.primaryWeaponAmmo = startWeapons.PrimaryWeaponAmmo.Value;
                this.maxConcurrentPrimaryWeaponAllowed = startWeapons.MaxConcurrentPrimaryWeaponAllowed;
                this.unlimitedPrimaryWeaponAmmos = startWeapons.UnlimitedPrimaryWeaponAmmos;
                this.equippedWeapon = this.primaryWeapon;
            }

            this.secondaryWeapon = startWeapons.SecondaryWeapon;
            if (this.secondaryWeapon != null)
            {
                this.secondaryWeaponAmmo = startWeapons.SecondaryWeaponAmmo.Value;
                this.maxConcurrentSecondaryWeaponAllowed = startWeapons.MaxConcurrentSecondaryWeaponAllowed;
                this.unlimitedSecondaryWeaponAmmos = startWeapons.UnlimitedSecondaryWeaponAmmos;   
            }
            float ammoRate = (float)primaryWeaponAmmo / (float)startWeapons.PrimaryWeaponAmmo.Value;
            Debug.Log($"WeaponsController {gameObject.name}: raising ammo event with ammo {ammoRate}");
            ammoEvent?.RaiseEvent(ammoRate);
            isInitialized = true;
        }

        public void HandleUseWeaponEvent(Vector2 tempPosition)
        {
            Debug.Log("WeaponsController: Use weapon event received.");
            if (equippedWeapon == null)
            {
                Debug.Log("WeaponsController: No weapon equipped.");
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(tempPosition);
            Debug.DrawRay(ray.origin, ray.direction * 200f, Color.blue, 20f);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, ClickableLayer))
            {
                return;
            }

            Vector3 targetPosition = hit.point;

            if (equippedWeapon == primaryWeapon)
            {
                UsePrimaryWeaponEvent.RaiseEvent(gameObject, targetPosition);
            }
            else if (equippedWeapon == secondaryWeapon)
            {
                UseSecondaryWeaponEvent.RaiseEvent(gameObject, targetPosition);
            }
            else
            {
                Debug.Log("WeaponsController: Equipped weapon does not match primary or secondary weapon.");
            }
        }

        public void HandlePrimaryWeaponEvent(GameObject source, Vector3 targetPosition)
        {
            // --------------------------------------------------------
            // 1. VÉRIFICATION DU TEMPS DE MISE EN OEUVRE
            // --------------------------------------------------------
            if (Time.time < nextPrimaryWeaponShotTime)
            {
                // Le tir est en cooldown / préparation.
                return;
            }
            
            // --------------------------------------------------------
            // 2. VÉRIFICATION DES MUNITIONS ET LIMITES CONCURRENTES
            // --------------------------------------------------------
            IWeaponLimiter limiter = source.GetComponent<IWeaponLimiter>();

            if (limiter == null) // Logique pour le joueur (source == gameObject)
            {
                 if (source != gameObject) return; // Si non IA et non local, ignorer
                 
                 if (primaryWeaponAmmo <= 0 && !unlimitedPrimaryWeaponAmmos) return;
                 
                 if (CurrentPrimaryWeaponInstancesActive.Count >= maxConcurrentPrimaryWeaponAllowed)
                 {
                     Debug.Log("WeaponsController (Player): Max concurrent primary weapon instances reached. Cannot fire.");
                     return;
                 }
            }
            else // Logique pour l'IA (source != gameObject)
            {
                 if (limiter.CurrentActiveTorpedos >= maxConcurrentPrimaryWeaponAllowed)
                 {
                     Debug.Log($"WeaponsController (AI): Max concurrent primary weapon instances reached ({limiter.CurrentActiveTorpedos} / {maxConcurrentPrimaryWeaponAllowed}). Cannot fire.");
                     return;
                 }
            }
            
            // --------------------------------------------------------
            // 3. TIR (Démarrage du temps de mise en œuvre)
            // --------------------------------------------------------
            
            // 🚨 Définir le prochain temps de tir disponible. Bloque immédiatement les tirs suivants.
            nextPrimaryWeaponShotTime = Time.time + primaryWeapon.TimeToTrigger;
            
            // Démarrer la coroutine pour attendre le temps de mise en œuvre (TimeToTrigger)
            StartCoroutine(TriggerPrimaryWeaponCoroutine(source, targetPosition, limiter));
        }

        private IEnumerator TriggerPrimaryWeaponCoroutine(GameObject source, Vector3 targetPosition, IWeaponLimiter limiter)
        {
            // Attente du temps de mise en œuvre (TimeToTrigger)
            yield return new WaitForSeconds(primaryWeapon.TimeToTrigger);

            // ⚠️ Le tir réel se produit après l'attente

            // Si c'est le joueur (limiter == null)
            if (limiter == null)
            {
                // Re-vérification de munitions pour le joueur juste avant le tir réel
                if (primaryWeaponAmmo <= 0 && !unlimitedPrimaryWeaponAmmos) yield break; 
                
                // Tir pour le joueur (sans limiteur d'IA)
                TriggerWeaponInstance(targetPosition, null);
                
                // Décrémentation des munitions du joueur (si non illimitées)
                if (!unlimitedPrimaryWeaponAmmos)
                {
                     primaryWeaponAmmo--;
                }
                float ammoRate = (float)primaryWeaponAmmo / (float)startWeapons.PrimaryWeaponAmmo.Value;
                Debug.Log($"WeaponsController: Primary weapon ammo: {primaryWeaponAmmo}");
                Debug.Log($"WeaponsController: Start value : {startWeapons.PrimaryWeaponAmmo.Value}");
                Debug.Log($"WeaponsController: Division : {ammoRate}");
                Debug.Log($"WeaponsController {gameObject.name}: raising ammo event with ammo { ammoRate}");
                ammoEvent.RaiseEvent(ammoRate);
            }
            // Si c'est l'IA (limiter != null)
            else
            {
                // Re-vérification de la limite concurrente pour l'IA (au cas où une autre torpille aurait été détruite pendant l'attente)
                // C'est facultatif mais plus sûr.
                if (limiter.CurrentActiveTorpedos >= maxConcurrentPrimaryWeaponAllowed) yield break;
                
                // Tir pour l'IA (avec limiteur)
                TriggerWeaponInstance(targetPosition, limiter);
            }
        }

        private void TriggerWeaponInstance(Vector3 targetPosition, IWeaponLimiter limiter)
        {
            GameObject weaponInstance = Instantiate(primaryWeapon.WeaponPrefab, torpedoLaunchPoint.position, torpedoLaunchPoint.rotation);
            Collider weaponCollider = weaponInstance.GetComponent<Collider>();
            Collider submarineCollider = GetComponent<Collider>();
            
            if (weaponCollider != null && submarineCollider != null)
            {
                Physics.IgnoreCollision(weaponCollider, submarineCollider, true);
            }
            
            BaseWeapon baseWeapon = weaponInstance.GetComponent<BaseWeapon>();
            
            if (limiter != null)
            {
                // 🚨 APPEL D'INIT IA : Passe l'interface de limiteur à la torpille
                baseWeapon?.Init(this, targetPosition, limiter); 
            }
            else
            {
                // APPEL D'INIT JOUEUR
                baseWeapon?.Init(this, targetPosition);
                CurrentPrimaryWeaponInstancesActive.Add(weaponInstance); 
            }
        }

        public void HandleSecondaryWeaponEvent(GameObject source, Vector3 targetPosition)
        {
             // 🚨 NOTE : Cette méthode doit être réécrite pour utiliser la logique de cooldown/limite concurrente
             // similaire à HandlePrimaryWeaponEvent et utiliser une coroutine.
             
             if (Time.time < nextSecondaryWeaponShotTime) return;

             // ... (vérifications de munitions et limite concurrente) ...
             
             nextSecondaryWeaponShotTime = Time.time + secondaryWeapon.TimeToTrigger;
             StartCoroutine(TriggerSecondaryWeaponCoroutine(source, targetPosition));
        }

        private IEnumerator TriggerSecondaryWeaponCoroutine(GameObject source, Vector3 targetPosition)
        {
             IWeaponLimiter limiter = source.GetComponent<IWeaponLimiter>();

             yield return new WaitForSeconds(secondaryWeapon.TimeToTrigger);

             // Logique de tir et décrémentation des munitions/compteur
             // ...
             
             // Exemple d'ancienne implémentation:
             /*
             GameObject weaponInstance = Instantiate(secondaryWeapon.WeaponPrefab, torpedoLaunchPoint.position, torpedoLaunchPoint.rotation);
             BaseWeapon baseWeapon = weaponInstance.GetComponent<BaseWeapon>();
             baseWeapon?.Init(this, targetPosition);
             secondaryWeaponAmmo--;
             CurrentSecondaryWeaponInstancesActive.Add(weaponInstance);
             */
        }

        private void Start()
        {
            if (startWeapons != null)
            {
                unlimitedPrimaryWeaponAmmos = startWeapons.UnlimitedPrimaryWeaponAmmos;
                primaryWeapon = startWeapons.PrimaryWeapon;
                if (startWeapons.PrimaryWeaponAmmo != null)
                {
                    primaryWeaponAmmo = startWeapons.PrimaryWeaponAmmo.Value;
                }
                maxConcurrentPrimaryWeaponAllowed = startWeapons.MaxConcurrentPrimaryWeaponAllowed;

                secondaryWeapon = startWeapons.SecondaryWeapon;
                unlimitedSecondaryWeaponAmmos = startWeapons.UnlimitedSecondaryWeaponAmmos;
                if (startWeapons.SecondaryWeaponAmmo != null)
                {
                    secondaryWeaponAmmo = startWeapons.SecondaryWeaponAmmo.Value;
                }
                maxConcurrentSecondaryWeaponAllowed = startWeapons.MaxConcurrentSecondaryWeaponAllowed;

                equippedWeapon = primaryWeapon;

                Debug.Log($"WeaponsController : set primary weapon ammo to {primaryWeaponAmmo}");
            }
        }
    }
}