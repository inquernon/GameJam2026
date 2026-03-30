using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Menu : MonoBehaviour
{
    public Nodo[] t;
    public float velocidad;
    int i;
    public GameObject zombiePR;
    public GameObject persinaje;
    public GameObject zombieNator;

    public Camera camara;
    public float fovInicial;
    public float fovFinal;
    public bool irFinal;

    private IEnumerator Start()
    {
        yield return null; 
        ScreenFader.Instance.FadeIn();
        yield return new WaitForSeconds(2);
    }

    private void Update()
    {
        t[i].transform.Translate(velocidad*Time.deltaTime*Vector3.up);
        if(zombieNator != null)
        {
            zombieNator.transform.Translate(-velocidad * Time.deltaTime * Vector3.forward);
        }
        if(t[i].transform.position.z < -60)
        {
            t[(i+1)%2].transform.parent = null;
            t[i].transform.parent = t[(i + 1) % 2].pivote;
            t[i].transform.localPosition = Vector3.zero;
            i = (i + 1) % 2;
        }
        if(irFinal)
        {
            camara.fieldOfView = Mathf.Lerp(camara.fieldOfView, fovFinal, Time.deltaTime);
        }
    }

    public void Zombitizar()
    {
        zombieNator = Instantiate(zombiePR, persinaje.transform.position + Vector3.forward*4, Quaternion.Euler(0,180,0));
        irFinal = true;
        Invoke("Fadear", 2);
    }

    public void ResetearScore()
    {
        PlayerPrefs.SetInt("score", 0);
    }
    public void Fadear()
    {
        ScreenFader.Instance.FadeOut();
    }
}
