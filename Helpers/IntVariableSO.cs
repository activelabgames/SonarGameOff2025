using UnityEngine;
using System;

[CreateAssetMenu(fileName = "IntVariableSO", menuName = "Helpers/Variables/Int Variable", order = 0)]
public class IntVariableSO : BaseVariableSO
{
    public event Action<int> OnValueChanged;
    [SerializeField] private int m_Value;

    public int Value
    {
        get => m_Value;
        set
        {
            if (m_Value != value)
            {
                m_Value = value;
                OnValueChanged?.Invoke(m_Value);
            }
        }
    }

    private void OnValidate()
    {
        Debug.Log("IntVariableSO: OnValidate called.");
        OnValueChanged?.Invoke(m_Value);
    }

    public override string ToString()
    {
        return "" + Value;
    }
}