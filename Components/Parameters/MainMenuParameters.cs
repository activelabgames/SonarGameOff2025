using Sonar;
using UnityEngine;

[CreateAssetMenu(fileName = "MainMenuParameters", menuName = "Sonar/Parameters/Main Menu")]
public class MainMenuParametersSO : BaseParametersSO
{
    public AudioClip AmbientMusic;
    public float AmbientMusicVolume;
}