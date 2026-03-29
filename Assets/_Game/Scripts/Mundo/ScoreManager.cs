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
    public FinalUIGameOver finalUIGameOver;
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
            scoreActual = pos;
            yield return new WaitForSeconds(0.2f);
            txtScore.text = pos.ToString(); // +" \n Best Score: " + hsGuardado;
            if((lineaRecord.position - trJugador.position).sqrMagnitude < 100)
                lineaRecord.position = new Vector3(trJugador.position.x,0,hsGuardado);
        }
        finalUIGameOver.ContarFinal();

        if (pos > hsGuardado )
        {
            PlayerPrefs.SetInt("score", pos);
        }
    }
}
