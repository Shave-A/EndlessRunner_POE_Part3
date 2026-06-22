using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ScoreEntry
{
    public string name;
    public int score;
}

[System.Serializable]
public class ScoreList
{
    public List<ScoreEntry> scores;
}

public class GetScores : MonoBehaviour
{
    string getScoresURL = "http://localhost/infiniteRunner/scores.php";

    public GameObject entryPrefab;
    public Transform contentParent;

    void Start()
    {
        StartCoroutine(FetchScores());
    }

    IEnumerator FetchScores()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(getScoresURL))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = "{\"scores\":" + www.downloadHandler.text + "}";
                ScoreList scoreList = JsonUtility.FromJson<ScoreList>(json);

                int rank = 1;
                foreach (ScoreEntry entry in scoreList.scores)
                {
                    GameObject row = Instantiate(entryPrefab, contentParent);
                    TMP_Text text = row.GetComponent<TMP_Text>();
                    text.text = rank + ".  " + entry.name + "  -  " + entry.score;
                    text.color = Color.black;
                    text.fontSize = 24;
                    rank++;
                }
            }
            else
            {
                Debug.LogError("Failed: " + www.error);
            }
        }
    }
}
