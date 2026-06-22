using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void StartGame()
    {
        Game.instance.score = 0;
        Game.instance.bossesDefeated = 0;
        SceneManager.LoadScene("Runner_WIP");
    }

    public void Mainmenu()
    {
        
        SceneManager.LoadScene("MainMenu");
    }
    public void DoExitGame()
    {
    
        Application.Quit();
    }

    public void LeaderBoard()
    {
        SceneManager.LoadScene("Leaderboard");
    }
    }
