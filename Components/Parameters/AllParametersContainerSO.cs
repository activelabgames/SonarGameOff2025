// AllParametersContainerSO.cs (Dans le dossier de votre projet)
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AllParameters", menuName = "Sonar/Parameters/All Parameters Container")]
public class AllParametersContainerSO : ScriptableObject
{
    public List<BaseParametersSO> GameSystemsParameters;
}