using UnityEngine;

namespace Sonar
{
    public abstract class BaseTorpedoDataSO : BaseWeaponDataSO
    {
        public float SoundIntensity;
        public AudioClip LaunchAudioClip;
        public AudioClip MovingAudioClip;
        public AudioClip ExplodingAudioClip;
        public float ExplodingAudioVolume;
        public AudioClip InactiveDestroyedAudioClip;
        public AudioClip LostAudioClip;
        public float Speed;
        public float TurnSpeed;
        public float Damage;
        public float Lifetime;
        public bool UnlimitedLifetime;
        public bool RemotelyDestroyable;
        public float AutomaticActivationDistance;
        public bool ActivatedAtLaunch = true;
        public float ExplosionDistance;
        public float ExplosionRange;
        public bool ExplodesOnCollisionWhenActivated = true;
        public float ApproachingDistance;
        public bool CollisionBetweenTorpedoes = false;

        [SerializeField] public Color ExplosionRangeSphereGizmoColor = new Color(1f, 0f, 0f, .4f);
        [SerializeField] public Color ExplosionRangeSphereGizmoContourColor = new Color(1f, 0f, 0f, 1f);

        private void OnEnable()
        {
            Enable();
        }
        private void OnDisable()
        {
            Disable();
        }
    }    
}
