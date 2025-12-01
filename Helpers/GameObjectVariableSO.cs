using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GameObjectVariableSO", menuName = "Helpers/Variables/GameObject Variable", order = 0)]
public class GameObjectVariableSO : BaseVariableSO
{
    public event Action<GameObject> OnValueChanged;
    [SerializeField] private GameObject m_Value;

    public GameObject Value
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
        Debug.Log("GameObjectVariableSO: OnValidate called.");
        OnValueChanged?.Invoke(m_Value);
    }

    public override string ToString()
    {
        return "" + Value;
    }
}