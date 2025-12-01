// IWeaponLimiter.cs

using UnityEngine;

namespace Sonar.AI
{
    public interface IWeaponLimiter
    {
        // Propriété pour suivre et modifier le nombre de torpilles actives
        int CurrentActiveTorpedos { get; set; }
    }
}