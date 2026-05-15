using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Game : MonoBehaviour
{
    public static Game instance;
    public int score;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Game instance created");
        }
        else
        {
            Debug.Log("Duplicate destroyed");
            Destroy(gameObject);
        }
    }


    public void ScoreUI()
    {
        scoreText.text = "Score: " + score;
    
    }
}
