using UnityEngine;

[CreateAssetMenu(fileName = "FloatWithPRefixVariableSO", menuName = "Helpers/Variables/Float With Prefix Variable", order = 0)]
public class FloatWithPrefixVariableSO : FloatVariableSO
{
    public string Prefix;

    public override string ToString()
    {
        return Prefix + base.ToString();
    }
}