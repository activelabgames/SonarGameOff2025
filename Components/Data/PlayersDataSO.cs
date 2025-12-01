using System.Collections.Generic;
using UnityEngine;

namespace Sonar
{
    [CreateAssetMenu(fileName = "PlayersDataSO", menuName = "Sonar/Data/Players", order = 0)]
    public class PlayersDataSO : BaseDataSO
    {
        [SerializeField] public List<PlayerDataSO> Players;
    }   
}
