using Sonar;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatParameters", menuName = "Sonar/Parameters/Combat")]
public class PlayersParametersSO : BaseParametersSO
{
    public PlayerParametersSO PlayerParameters;
    public PlayerParametersSO EnemyParameters;
}