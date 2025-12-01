using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace Sonar
{
    [RequireComponent(typeof(Collider),typeof(NavMeshAgent) )]
    public class SimpleTorpedo: BaseWeapon
    {
        [SerializeField] private bool isActivated = false;
        [SerializeField] private bool isApproaching = false;
        private SimpleTorpedoDataSO torpedoData;
        NavMeshAgent navMeshAgent;

        VisualEffect torpedoExplosion;
        AudioSource audioSource;

        [SerializeField] private float lifetimeTimer = 0.0f;

        [SerializeField] private bool explosionRangeGizmoEnabled = true;

        private bool isExploding = false;

        private EquipmentController equipmentController;
        [SerializeField] private GameObjectEventChannelSO ControllerDestroyedEvent;

        private void Awake()
        {
            equipmentController = GetComponent<EquipmentController>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            torpedoData = (SimpleTorpedoDataSO)weaponData;
            isActivated = torpedoData.ActivatedAtLaunch;
            torpedoExplosion = GetComponent<VisualEffect>();
            audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                Debug.Log("Weapon: setting audio clip and volume.");
                //audioSource.resource = torpedoData.ExplodingAudioClip;
                //audioSource.volume = torpedoData.ExplodingAudioVolume;
            }
            
        }
        private void Start()
        {
            lifetimeTimer = 0.0f;
            navMeshAgent.speed = torpedoData.Speed;
            navMeshAgent.angularSpeed = torpedoData.TurnSpeed;
        }

        private void Explode()
        {
            Debug.Log("Weapon: Torpedo is exploding.");
            // TODO : add damages system
            if (weaponsController == null)
            {
                Debug.Log("Weapon: weaponsController is null, cannot proceed with explosion.");
                return;
            }
            if(equipmentController != null)
            {
                Debug.Log("Weapon: Raising ControllerDestroyedEvent for equipmentController.");
                ControllerDestroyedEvent?.RaiseEvent(equipmentController.gameObject);                
            }
            weaponsController.CurrentPrimaryWeaponInstancesActive.Remove(gameObject);
            audioSource.Play();
            torpedoExplosion?.SendEvent("OnExplode");
            Debug.Log($"Weapon: Playing explosion sound {audioSource.resource} at volume {audioSource.volume}");
            Destroy(gameObject, 3.0f);
        }

        private void SelfBreak()
        {
            Debug.Log("Weapon: Torpedo is broken.");
            // TODO : add animation system
            if(weaponsController == null)
            {
                return;
            }
            weaponsController.CurrentPrimaryWeaponInstancesActive.Remove(gameObject);
            Destroy(gameObject);
        }

        private void Update()
        {
            Debug.Log($"Weapon: targetPosition: {targetPosition}");

            if (isInitialized)
            {
                Debug.Log($"Weapon: setting torpedo navmeshagent destination to {targetPosition}.");
                navMeshAgent?.SetDestination(targetPosition);
            }

            lifetimeTimer += Time.deltaTime;
            if (!torpedoData.UnlimitedLifetime && lifetimeTimer >= torpedoData.Lifetime)
            {
                Debug.Log("Weapon: Torpedo reached its lifetime and is destroying.");
                if (weaponsController != null)
                {
                    weaponsController.CurrentPrimaryWeaponInstancesActive.Remove(gameObject);
                }
                Destroy(gameObject);
            }
            //Debug.Log($"Weapon: targetPosition - transform.position = {targetPosition - transform.position}");
            // Reorient the torpedo in the direction of its destination
            /*if (transform.forward != (targetPosition - transform.position))
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(targetPosition.x, 0, targetPosition.y) - transform.position);
                Debug.Log($"Weapon: Calculating new rotation: {targetRotation}");
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, torpedoData.TurnSpeed * Time.deltaTime);
            }
            else
            {
                Debug.Log($"Weapon: Good rotation !!!");
            }*/

            if (!isActivated && weaponsController != null && Vector3.Distance(transform.position, weaponsController.transform.position) >= torpedoData.AutomaticActivationDistance)
            {
                Debug.Log("Weapon: Activating torpedo");
                Collider weaponCollider = GetComponent<Collider>();
                Collider submarineCollider = weaponsController.GetComponent<Collider>();
                if (weaponCollider != null && submarineCollider != null)
                {
                    Physics.IgnoreCollision(weaponCollider, submarineCollider, false);
                }
                isActivated = true;
            }

            Debug.Log($"Weapon: Position = {transform.position}");
            Debug.Log($"Weapon: distance from target = {Vector3.Distance(transform.position, targetPosition)} (Explosion distance: {torpedoData.ExplosionDistance})");

            if (Vector3.Distance(transform.position, targetPosition) <= torpedoData.ApproachingDistance)
            {
                Debug.Log("Weapon: enabling approaching gizmos");
                isApproaching = true;
            }            

            if (Vector3.Distance(transform.position, targetPosition) <= torpedoData.ExplosionDistance)
            {
                Debug.Log("Weapon: torpedo reached its destination and will explode.");
                if (!isExploding)
                {
                    isExploding = true;
                    Explode();
                    ApplyDamages();
                }
            }

            //rb.linearVelocity = transform.forward * torpedoData.Speed;
        }

        private void ApplyDamages()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, torpedoData.ExplosionRange);

            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    hit.TryGetComponent<HealthComponent>(out HealthComponent otherHealthComponent);
                    if (otherHealthComponent != null)
                    {
                        float distance = Vector3.Distance(otherHealthComponent.transform.position, transform.position);
                        Debug.Log($"Weapon: target is at distance {distance}");
                        float damage = CalculateDamage(distance);
                        Debug.Log($"Weapon: applying damage {damage}");
                        otherHealthComponent.TakeDamage(damage);
                    }
                }
            }
        }

        private float CalculateDamage(float distance)
        {
            float damage = (-torpedoData.Damage / torpedoData.ExplosionRange) * distance + torpedoData.Damage;
            Debug.Log($"Weapon: calculating damae. Formula: (-{torpedoData.Damage}/{torpedoData.ExplosionRange}) * {distance} + {torpedoData.Damage}");
            return damage;
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Weapon: OnTriggerEnter with {other}");
            if (weaponsController != null && other.gameObject == weaponsController.gameObject && !isActivated)
            {
                Debug.Log("Weapon: Collision with our launcher submarine while inactive. Ignored.");
                return;
            }

            if (!torpedoData.ExplodesOnCollisionWhenActivated)
            {
                Debug.Log($"Weapon: Collision with {other.gameObject.name} but ExplodesOnCollisionWhenActivated is false.");
                return;
            }

            if (other.gameObject.layer == gameObject.layer && !torpedoData.CollisionBetweenTorpedoes)
            {
                Debug.Log("Weapon: ignoring collision between two torpedoes");
                return;
            }

            Debug.Log($"Weapon: collision with {other.gameObject.name}");

            if (isActivated)
            {
                Debug.Log($"Weapon: Active torpedo collided with {other.gameObject.name} and is exploding.");
                /*collision.gameObject.TryGetComponent<HealthComponent>(out HealthComponent otherHealthComponent);
                if (otherHealthComponent != null)
                {
                    otherHealthComponent.TakeDamage(torpedoData.Damage);
                }*/
                if (!isExploding)
                {
                    Explode();
                    ApplyDamages();                    
                }

            }
            else
            {
                Debug.Log($"Weapon: Inactive torpedo collided with {other.gameObject.name} and broke.");
                SelfBreak();
            }
        }

        private void OnCollisionEnter(Collision other) {
            Debug.Log("Weapon: OnCollisionEnter");
        }

        private void OnDrawGizmos()
        {
            if (explosionRangeGizmoEnabled && isApproaching)
            {

                // 1. Définir la couleur du Gizmo
                // Utilisation d'une couleur semi-transparente pour mieux visualiser
                Gizmos.color = torpedoData.ExplosionRangeSphereGizmoColor;

                // 2. Dessiner la sphère pleine (ou DrawWireSphere pour un contour)
                // La position est celle du GameObject. Le rayon est dans le ScriptableObject.
                Gizmos.DrawSphere(transform.position, torpedoData.ExplosionRange);

                // Optionnel : Dessiner le contour pour une meilleure visibilité
                //Gizmos.color = torpedoData.ExplosionRangeSphereGizmoContourColor;
                //Gizmos.DrawSphere(transform.position, torpedoData.ExplosionRange);
            }
        }
    }
}
