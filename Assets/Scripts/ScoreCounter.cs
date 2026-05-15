using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
        


public class ScoreCounter : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Game.instance.score++;
            Game.instance.ScoreUI();
            Debug.Log("score Increased");
        }
    }

}
