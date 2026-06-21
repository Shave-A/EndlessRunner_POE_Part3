using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

public class DataInput : MonoBehaviour
{
    string createUserURL = "http://localhost/infiniteRunner/InsertUsers.php";

    public TMP_InputField nameInputField;
    public Button submitButton;

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmitClicked);
    }

    void OnSubmitClicked()
    {
        string name = nameInputField.text;

        if (string.IsNullOrEmpty(name))
        {
            Debug.Log("Name is empty!");
            return;
        }

        StartCoroutine(CreateScore(name));
    }

    IEnumerator CreateScore(string name)
    {

        Debug.Log("Sending - Name: " + name + " Score: " + Game.instance.score);
        WWWForm form = new WWWForm();
        form.AddField("username", name);
        form.AddField("score", Game.instance.score);

        using (UnityWebRequest www = UnityWebRequest.Post(createUserURL, form))
        {
            yield return www.SendWebRequest();

            Debug.Log("Response code: " + www.responseCode);
            Debug.Log("Response text: " + www.downloadHandler.text);

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Success: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Failed: " + www.error);
            }
        }
    }

}