public class GameMessage
{
    public string Message;
    public bool PauseGame;
    public float DisplayTime;

    public GameMessage(string message, bool pauseGame, float displayTime)
    {
        Message = message;
        PauseGame = pauseGame;
        DisplayTime = displayTime;
    }
}