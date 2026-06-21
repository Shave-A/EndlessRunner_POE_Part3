using UnityEngine;

public class DataInput : MonoBehaviour
{
    string createUserURL = "http://localhost/infiniteRunner/InsertUsers.php";
    public void CreateScore(string name)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", name);
        form.AddField("score", Game.instance.score);

        WWW www = new WWW(createUserURL, form);
    }
}
