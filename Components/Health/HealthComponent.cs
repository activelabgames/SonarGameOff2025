using UnityEngine;
using UnityEditor;

namespace Sonar
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private HealthCharacteristicsSO _healthCharacteristics;

        [SerializeField] private float _currentHealth;

        [SerializeField] private FloatEventChannelSO _healthUpdated = default;
        [SerializeField] private GameObjectEventChannelSO dieEvent = default;
        [SerializeField] private GameObjectEventChannelSO ControllerDestroyedEvent;

        public HealthCharacteristicsSO HealthCharacteristics => _healthCharacteristics;
        public float CurrentHealth => _currentHealth;

        [SerializeField] private bool isInitialized = false;

        public void Init(HealthCharacteristicsSO healthCharacteristics)
        {
            _healthCharacteristics = healthCharacteristics;
            _currentHealth = _healthCharacteristics.MaxHealth;
            isInitialized = true;
        }

        public void SetHealthUpdatedEvent(FloatEventChannelSO healthUpdatedEvent)
        {
            _healthUpdated = healthUpdatedEvent;
        }

        public void TakeDamage(float damageToApply)
        {
            if (_healthCharacteristics.Invulnerable)
            {
                //Debug.Log("Invulnerable");
                return;
            }
            _currentHealth -= damageToApply;

            Debug.Log($"HealthComponent: {gameObject.name} Taking damage!!!");

            if (_currentHealth <= 0)
            {
                Die();
            }

            if (_healthUpdated)
                _healthUpdated.RaiseEvent(_currentHealth);
        }

        private void Die()
        {
            // Send die event
            ControllerDestroyedEvent?.RaiseEvent(gameObject);
            dieEvent?.RaiseEvent(gameObject);
            //Destroy(this.gameObject);
        }
        
        public void GiveHealth(float healthToApply)
            {
                _currentHealth += healthToApply;

                //Debug.Log("Restoring health!!!");

                if (_currentHealth >= _healthCharacteristics.MaxHealth) _currentHealth = _healthCharacteristics.MaxHealth;

                if (_healthUpdated)
                    _healthUpdated.RaiseEvent(_currentHealth);
            }
    }
/*
    [CustomEditor(typeof(HealthComponent))]
    public class HealthComponentEditor : Editor 
    {
        private Editor editorInstance;
        private void OnEnable() 
        {
            editorInstance = null;
        }
        public override void OnInspectorGUI() 
        {
            HealthComponent healthComponent = (HealthComponent) target;
            if(editorInstance == null)
                editorInstance = Editor.CreateEditor(healthComponent.HealthCharacteristics);
            base.OnInspectorGUI();
            if(editorInstance != null)
            {
                editorInstance.DrawDefaultInspector();
            }
        }
    }    */
}
