using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public float delay = 2;
    public void Jugar()
    {
        StartCoroutine(InicioJugar());
    }
    public IEnumerator InicioJugar()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(1);
    }

    public void Salir()
    {
        Application.Quit();
    }
}