using UnityEngine;
using UnityEngine.UI;

namespace Sonar
{
    public class BoolVariableButton : MonoBehaviour
    {
        [SerializeField] private EmptyEventChannelSO buttonEvent;
        [SerializeField] private Outline buttonOutline;
        [SerializeField] private BoolVariableSO variable;

        [SerializeField] private Color variableTrueBorderColor;
        [SerializeField] private Color variableFalseBorderColor;

        [SerializeField] private Vector2 variableTrueBorderSize;
        [SerializeField] private Vector2 variableFalseBorderSize;

        private Color getCurrentColor()
        {
            if(variable)
            {
                return variableTrueBorderColor;
            }
            else
            {
                return variableFalseBorderColor;
            }
        }

        private Vector2 getCurrentBorderSize()
        {
            if(variable.Value)
            {
                return variableTrueBorderSize;
            }
            else
            {
                return variableFalseBorderSize;
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            buttonOutline.effectColor = getCurrentColor();
            buttonOutline.effectDistance = getCurrentBorderSize();
        }

        // Update is called once per frame
        private void Update()
        {
            buttonOutline.effectColor = getCurrentColor();
            buttonOutline.effectDistance = getCurrentBorderSize();
        }

        private void HandleButtonClick()
        {
            buttonEvent?.RaiseEvent();
        }
    }    
}
