using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text txtScore;
    public Transform trJugador;
    public int hsGuardado;
    public Transform lineaRecord;
    public Text txtHScore;
    public int scoreActual;
    public int bonus = 0;
    public FinalUIGameOver finalUIGameOver;
    public bool recordPersonal = false, recordGlobal = false;

    public static ScoreManager singleton;

    private void Awake()
    {
        singleton = this;
    }
    IEnumerator Start()
    {
        int pos = 0;
        hsGuardado = PlayerPrefs.GetInt("score", 0);
        //lineaRecord.Translate(0, hsGuardado, 0);
        lineaRecord.position = new Vector3(0,0,hsGuardado);
        txtHScore.text = hsGuardado.ToString();
        yield return new WaitUntil(() => GameManager.Instance.EstaJugando);
        
        
        while (GameManager.Instance.EstaJugando)
        {
            pos = Mathf.FloorToInt(trJugador.position.z);
            scoreActual = pos + bonus;
            yield return new WaitForSeconds(0.2f);
            txtScore.text = scoreActual.ToString() + " m";
            if((lineaRecord.position.z < trJugador.position.z))
                lineaRecord.position = new Vector3(trJugador.position.x,0,hsGuardado);
        }
        finalUIGameOver.ContarFinal();

        if (pos > hsGuardado )
        {
            recordPersonal = true;
            PlayerPrefs.SetInt("score", pos);
        }
    }
    public void AumentarBonus(int cuanto)
    {
        bonus += cuanto;
    }
}
