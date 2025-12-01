using UnityEngine;

[CreateAssetMenu(fileName = "IntWithPRefixVariableSO", menuName = "Helpers/Variables/Int With Prefix Variable", order = 0)]
public class IntWithPrefixVariableSO : IntVariableSO
{
    public string Prefix;

    public override string ToString()
    {
        return Prefix + base.ToString();
    }
}