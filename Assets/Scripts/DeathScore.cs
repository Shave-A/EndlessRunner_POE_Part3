using TMPro;
using UnityEngine;

public class DeathScore : MonoBehaviour
{
    
    
        public TextMeshProUGUI finalScoreText;

        private void Start()
        {
        Debug.Log("Score from Game.instance: " + Game.instance.score);
        finalScoreText.text = "Score: " + Game.instance.score;
        }
    
}
