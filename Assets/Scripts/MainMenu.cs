using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void StartGame()
    {
        Game.instance.score = 0;
        SceneManager.LoadScene("Runner_WIP");
    }
    public void DoExitGame()
    {
    
        Application.Quit();
    }
    }
