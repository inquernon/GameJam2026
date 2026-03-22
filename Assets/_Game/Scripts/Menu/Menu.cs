using System.Collections;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public Nodo[] t;
    public float velocidad;
    int i;
    float limite;
    public UIAutoAnimation boton1;
    public UIAutoAnimation boton2;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        boton1.EntranceAnimation();
        yield return new WaitForSeconds(.5f);
        boton2.EntranceAnimation();
    }

    private void Update()
    {
        t[i].transform.Translate(velocidad*Time.deltaTime*Vector3.up);
        if(t[i].transform.position.z < -60)
        {
            t[(i+1)%2].transform.parent = null;
            t[i].transform.parent = t[(i + 1) % 2].pivote;
            t[i].transform.localPosition = Vector3.zero;
            i = (i + 1) % 2;
        }
        
    }
}
