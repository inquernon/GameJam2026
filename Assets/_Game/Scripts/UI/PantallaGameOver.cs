using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PantallaGameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoScoreFinal;

    public void Mostrar(int scoreFinal)
    {
        gameObject.SetActive(true);
        textoScoreFinal.text = "Score: " + scoreFinal;
    }

    public void Reintentar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}