using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public float delay = 2;
    public float delay2 = 1;
    public int cual = 1;
   
    public void Jugar()
    {
        StartCoroutine(InicioJugar());
        ScreenFader.Instance.FadeIn();
    }
    public void Menu()
    {
        StartCoroutine(InicioMenu());
    }
    public IEnumerator InicioJugar()
    {
        yield return new WaitForSeconds(delay);
        ScreenFader.Instance?.FadeOut();
        yield return new WaitForSeconds(delay2);
        SceneManager.LoadScene(cual);
    }

    public IEnumerator InicioMenu()
    {
        yield return new WaitForSeconds(delay);
        ScreenFader.Instance?.FadeOut();
        yield return new WaitForSeconds(delay2);
        SceneManager.LoadScene("Menu");
    }
    public void Salir()
    {
        Application.Quit();
    }
}