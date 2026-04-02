using UnityEngine;
using UnityEngine.UI;

public class ItemTablaGlobal : MonoBehaviour
{
    public Image imNumero;
    public Color colorLocal;
    public Text txtScore, txtNumero, txtNombre;
    string idUSuaruio;
    public void Inicializar(string nombre, int score, string id, int numero)
    {

        idUSuaruio = PlayerPrefs.GetString("usid", "");
        if (idUSuaruio.Length < 6)
        {
            for (int i = 0; i < 8; i++)
            {
                idUSuaruio += Random.Range(1, 9).ToString();
            }
            PlayerPrefs.SetString("usid", idUSuaruio);
        }

        txtNumero.text = numero.ToString("00");
        txtScore.text = score.ToString();
        txtNombre.text = nombre;
        if (idUSuaruio.Equals(id))
        {
            imNumero.color = colorLocal;
        }
    }
}
