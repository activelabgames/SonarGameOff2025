using Sonar;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundParameters", menuName = "Sonar/Parameters/Round")]
public class RoundParametersSO : BaseParametersSO
{
    public int EnemiesNumber;
    public int RoundDuration;
    public bool UnlimitedTime;
    public float MinEnemySpawnDistanceFromPlayer = 50.0f;

    public AudioClip MainAmbience;
    public float MainAmbienceVolume;
}