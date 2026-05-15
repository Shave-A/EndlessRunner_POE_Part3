using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void StartGame()
    {
        
        SceneManager.LoadScene("Runner_WIP");
    }
    public void DoExitGame()
    {
        
        Application.Quit();
    }
    }
