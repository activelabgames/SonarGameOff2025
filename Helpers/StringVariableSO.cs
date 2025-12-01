using UnityEngine;

[CreateAssetMenu(fileName = "StringVariableSO", menuName = "Helpers/Variables/String Variable", order = 0)]
public class StringVariableSO : BaseVariableSO
{
    public string Value;

    public override string ToString()
    {
        return Value;
    }
}