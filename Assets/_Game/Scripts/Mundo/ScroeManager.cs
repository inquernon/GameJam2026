using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScroeManager : MonoBehaviour
{
    public Text txtScore;
    public Transform trJugador;
    public int hsGuardado;
    IEnumerator Start()
    {
        int pos = 0;
        hsGuardado = PlayerPrefs.GetInt("score", 0);
        while (GameManager.Instance.vivo)
        {
            pos = Mathf.FloorToInt(trJugador.position.z);
            yield return new WaitForSeconds(0.2f);
            txtScore.text = pos + " - " + hsGuardado;
        }
        if (pos > hsGuardado )
        {
            PlayerPrefs.SetInt("score", pos);
        }
    }
}
