using Sonar;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStateSO", menuName = "Sonar/Game/State", order = 0)]
public class GameStateSO : ScriptableObject
{
    public string sceneName;

    public void LoadScene(GameManager gameManager)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void Behave()
    {

    }
}