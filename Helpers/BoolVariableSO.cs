using UnityEngine;

[CreateAssetMenu(fileName = "BoolVariableSO", menuName = "Helpers/Variables/Bool Variable", order = 0)]
public class BoolVariableSO : BaseVariableSO
{
    public bool Value;

    public override string ToString()
    {
        return "" + Value;
    }
}