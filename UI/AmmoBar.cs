using UnityEngine;
using UnityEngine.UI;


namespace Sonar
{
    public class AmmoBar : MonoBehaviour
    {
        public Image image;        
        [SerializeField] private FloatEventChannelSO _ammosUpdated = default;
        [SerializeField] private Color manyColor;
        [SerializeField] private float mediumLimit;
        [SerializeField] private Color mediumColor;
        [SerializeField] private float fewLimit;
        [SerializeField] private Color fewColor;

        private float ammos;

        private void OnEnable()
        {
            if (_ammosUpdated != null)
            {
                _ammosUpdated.OnEventRaised += HandleAmmosUpdatedEvent;
            }
        }

        private void OnDisable()
        {
            if (_ammosUpdated != null)
            {
                _ammosUpdated.OnEventRaised -= HandleAmmosUpdatedEvent;
            }
        }

        private void HandleAmmosUpdatedEvent(float ammosRate)
        {
            if(image == null)
            {
                Debug.Log("AmmosBar image is null");
                return;
            }
            this.ammos = ammosRate;
            Debug.Log($"AmmoBar: received ammos: {ammosRate}, will fill with {ammosRate}");
            image.fillAmount = ammosRate;
            //image.color = GetCurrentColor();
        }

        private void Update()
        {
            Debug.Log("AmmoBar: update");
        }

        private Color GetCurrentColor()
        {
            if (ammos > mediumLimit)
            {
                return manyColor;
            }
            
            if (ammos > fewLimit)
            {
                return mediumColor;
            }

            return fewColor;
        }
    }    
}
