using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic; 

[System.Serializable]
public class LeaderboardEntry
{
    public int position;
    public string userId;
    public string nombre;
    public int score;
}

[System.Serializable]
public class LeaderboardResponse
{
    public bool success;
    public int count;
    public List<LeaderboardEntry> leaderboard;
}

public class LeaderboardDisplay : MonoBehaviour
{
    public string getUrl = "https://tu-servidor.com/get_leaderboard.php";
    public bool cargando = false;
    public LeaderboardResponse total;

    public void GetLeaderboard()
    {
        cargando=true;
        StartCoroutine(GetLeaderboardCoroutine());
    }

    private IEnumerator GetLeaderboardCoroutine()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(getUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(www.downloadHandler.text);
                total = response;
                //foreach (var entry in response.leaderboard)
                //{
                //    Debug.Log($"{entry.position}. [{entry.userId}] {entry.nombre}: {entry.score}");
                //}
            }
        }
        cargando = false ;
    }
}