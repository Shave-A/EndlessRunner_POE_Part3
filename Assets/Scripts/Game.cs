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
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ScoreUI()
    {
        scoreText.text = "Score: " + score;
    
    }
}
