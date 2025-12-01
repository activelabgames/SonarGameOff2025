using System.Collections.Generic;
using Sonar;
using UnityEngine;

[CreateAssetMenu(fileName = "GameParameters", menuName = "Sonar/Parameters/Game")]
public class GameParametersSO : BaseParametersSO
{
    public MainMenuParametersSO MainMenuParameters;
    public List<RoundParametersSO> RoundParameters;
    public PlayersParametersSO PlayersParameters;
    public ScoreParametersSO ScoreParameters;
    
}