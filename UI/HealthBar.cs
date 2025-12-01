using UnityEngine;
using UnityEngine.UI;


namespace Sonar
{
    public class HealthBar : MonoBehaviour
    {
        public Image image;        
        [SerializeField] private FloatEventChannelSO _healthUpdated = default;
        [SerializeField] private Color healthyColor;
        [SerializeField] private int woundedLimit;
        [SerializeField] private Color woundedColor;
        [SerializeField] private int gravelyWoundedLimit;
        [SerializeField] private Color gravelyWoundedColor;

        private float health = 100f;

        private void OnEnable()
        {
            if (_healthUpdated != null)
            {
                _healthUpdated.OnEventRaised += HandleHealthUpdatedEvent;
            }
        }

        private void OnDisable()
        {
            if (_healthUpdated != null)
            {
                _healthUpdated.OnEventRaised -= HandleHealthUpdatedEvent;
            }
        }

        private void HandleHealthUpdatedEvent(float health)
        {
            if(image == null)
            {
                Debug.Log("HealthBar image is null");
                return;
            }
            this.health = health;
            image.fillAmount = health / 100;
            image.color = GetCurrentColor();
        }

        private Color GetCurrentColor()
        {
            if (health > woundedLimit)
            {
                return healthyColor;
            }
            
            if (health > gravelyWoundedLimit)
            {
                return woundedColor;
            }

            return gravelyWoundedColor;
        }
    }    
}
