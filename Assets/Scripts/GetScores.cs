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
                Debug.Log("Response: " + www.downloadHandler.text);

                string json = "{\"scores\":" + www.downloadHandler.text + "}";
                Debug.Log("Parsed JSON: " + json);
                ScoreList scoreList = JsonUtility.FromJson<ScoreList>(json);
                Debug.Log("Score count: " + scoreList.scores.Count);

                int rank = 1;
                foreach (ScoreEntry entry in scoreList.scores)
                {
                    GameObject row = Instantiate(entryPrefab, contentParent);
                    TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
                    texts[0].text = rank.ToString();
                    texts[1].text = entry.name;
                    texts[2].text = entry.score.ToString();

                    Debug.Log("Set texts: " + texts[0].text + " " + texts[1].text + " " + texts[2].text);
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