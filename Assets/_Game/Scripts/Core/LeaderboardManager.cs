using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LeaderboardManager : MonoBehaviour
{
    public string saveUrl = "https://tu-servidor.com/save_score.php";

    public void SaveScore(string userId, string nombre, int score)
    {
        StartCoroutine(SaveScoreCoroutine(userId, nombre, score));
    }

    private IEnumerator SaveScoreCoroutine(string userId, string nombre, int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("nombre", nombre);
        form.AddField("score", score);

        using (UnityWebRequest www = UnityWebRequest.Post(saveUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Respuesta: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }
}

// Uso:
// SaveScore("12345678", "Player1", 1500);