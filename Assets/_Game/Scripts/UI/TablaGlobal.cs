using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TablaGlobal : MonoBehaviour
{
    public GameObject prItemTabla;
    public Transform padre;
    public LeaderboardDisplay leaderboardDisplay;
    public Text txtMenu;
    public GameObject iconoRankeado;
    string idUSuaruio;
    bool encontrado = false;
    IEnumerator Start()
    {
        leaderboardDisplay.GetLeaderboard();
        yield return null;


        idUSuaruio = PlayerPrefs.GetString("usid", "");
        if (idUSuaruio.Length < 6)
        {
            for (int i = 0; i < 8; i++)
            {
                idUSuaruio += Random.Range(1, 9).ToString();
            }
            PlayerPrefs.SetString("usid", idUSuaruio);
        }

        while (leaderboardDisplay.cargando)
        {
            yield return null;
        }
        if(leaderboardDisplay.total.leaderboard != null)
        {
            LeaderboardEntry le;
            GameObject go;
            ItemTablaGlobal itemTabla;
            for (int i = 0; i < leaderboardDisplay.total.leaderboard.Count; i++)
            {
                le = leaderboardDisplay.total.leaderboard[i];
                go = Instantiate(prItemTabla, padre);
                itemTabla = go.GetComponent<ItemTablaGlobal>();
                itemTabla.Inicializar(le.nombre, le.score, le.userId, i+1);
                go.SetActive(true);
                if(!encontrado && le.userId.Equals(idUSuaruio))
                {
                    txtMenu.text = (i+1).ToString("00");
                    encontrado = true;
                }
            }
            iconoRankeado.SetActive(encontrado);
        }
    }
    public void Inicializar()
    {
        //StartCoroutine(Iniciar());
    }
    IEnumerator Iniciar()
    {
        yield return null;
        while (leaderboardDisplay.cargando)
        {
            yield return null;
        }
        if(leaderboardDisplay.total.leaderboard != null)
        {
            LeaderboardEntry le;
            GameObject go;
            ItemTablaGlobal itemTabla;
            for (int i = 0; i < leaderboardDisplay.total.leaderboard.Count; i++)
            {
                le = leaderboardDisplay.total.leaderboard[i];
                go = Instantiate(prItemTabla, padre);
                itemTabla = go.GetComponent<ItemTablaGlobal>();
                itemTabla.Inicializar(le.nombre, le.score, le.userId, i+1);
                go.SetActive(true);
            }
        }
    }

}
