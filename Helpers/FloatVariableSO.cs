using UnityEngine;

[CreateAssetMenu(fileName = "FloatVariableSO", menuName = "Helpers/Variables/Float Variable", order = 0)]
public class FloatVariableSO : BaseVariableSO
{
    public float Value;

    public override string ToString()
    {
        return "" + Value;
    }
}