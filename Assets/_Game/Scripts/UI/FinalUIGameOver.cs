using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FinalUIGameOver : MonoBehaviour
{
    public Text txtScore;
    public float esperas = 0.1f;

    public ScoreManager scoreManager;
    public LeaderboardDisplay leaderboardDisplay;
    public LeaderboardManager leaderboardManager;
    public InputField inputScoreGlobal;
    string idUSuaruio;
    void Start()
    {
        leaderboardDisplay.GetLeaderboard();
        idUSuaruio = PlayerPrefs.GetString("usid", "");
        if(idUSuaruio.Length < 6)
        {
            for(int i = 0; i < 8; i++)
            {
                idUSuaruio += Random.Range(1, 9).ToString();
            }
            PlayerPrefs.SetString("usid", idUSuaruio);
        }
        inputScoreGlobal.text = PlayerPrefs.GetString("nombre");
    }

    public void ContarFinal()
    {
        leaderboardDisplay.GetLeaderboard();
        StartCoroutine(VerificarTablaGlobal());
        StartCoroutine(SumaFinal());
    }

    public void GuardarScore()
    {
        leaderboardManager.SaveScore
        (
            idUSuaruio, 
            inputScoreGlobal.text.Length >0? inputScoreGlobal.text:"No name", 
            scoreManager.scoreActual
        );

        if(inputScoreGlobal.text.Length > 2) PlayerPrefs.SetString("nombre", inputScoreGlobal.text);
    }

    public IEnumerator VerificarTablaGlobal()
    {
        float tiempoEsperaFinal = Time.time + 5f;
        inputScoreGlobal.gameObject.SetActive(false);
        while (Time.time < tiempoEsperaFinal || leaderboardDisplay.cargando)
        {
            yield return null;
        }
        if (leaderboardDisplay.cargando)
        {
            inputScoreGlobal.gameObject.SetActive(false);
        }
        else
        {
            if(leaderboardDisplay.total.leaderboard.Count < 20)
            {
                inputScoreGlobal.gameObject.SetActive(true);
            }
            else
            {
                if (leaderboardDisplay.total.leaderboard[leaderboardDisplay.total.leaderboard.Count-1].score <=scoreManager.scoreActual)
                {
                    inputScoreGlobal.gameObject.SetActive(true);
                }
                else
                {
                    inputScoreGlobal.gameObject.SetActive(false);
                }
            }
        }
    }

    public IEnumerator SumaFinal()
    {
        if (scoreManager.scoreActual > 100) esperas /= 2f;
        if (scoreManager.scoreActual > 300) esperas /= 2f;
        if (scoreManager.scoreActual > 500) esperas /= 2f;
        if (scoreManager.scoreActual > 600) esperas /= 2f;
        for (int i = 0; i <= scoreManager.scoreActual; i++)
        {
            if (scoreManager.scoreActual > 100 && scoreManager.scoreActual - i - 1 > 5) i++;
            if (scoreManager.scoreActual > 300 && scoreManager.scoreActual - i - 1 > 5) i++;
            if (scoreManager.scoreActual > 800 && scoreManager.scoreActual - i - 1 > 5) i++;
            if (scoreManager.scoreActual > 1000 && scoreManager.scoreActual - i - 1 > 5) i++;
            txtScore.text = i.ToString() + "/" + scoreManager.hsGuardado;
            yield return new WaitForSeconds(esperas);
        }
    }
}
