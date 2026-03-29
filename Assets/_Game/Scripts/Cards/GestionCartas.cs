using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GestionCartas : MonoBehaviour
{
    public static GestionCartas singleton;
    public CartaPoder[] cartasPoder;
    public Animator animCarta;
    public int zombiesPorCarta;
    public int zombiesActual;
    public MeshRenderer mshCarta;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        singleton = this;
    }

    public void MurioZombie()
    {
        zombiesActual++;
        if( zombiesActual >= zombiesPorCarta)
        {
            zombiesActual = 0;
            GanarCarta();
        }
        GameUI.singleton.ActualizarZombies(zombiesActual,zombiesPorCarta);
    }

    public void GanarCarta()
    {
        StartCoroutine(CargandoCarta());
    }

    IEnumerator CargandoCarta()
    {
        int cual = Random.Range(0, cartasPoder.Length);
        mshCarta.sharedMaterial = cartasPoder[cual].material;
        animCarta.SetTrigger("mostrar");
        yield return new WaitForSeconds(2);
        cartasPoder[cual].eventoActivar.Invoke();

    }

    public void ActivarIman()
    {
        EnemyMagnet.singleton.ActivarIman();
    }

    public void ActivarMasScore(int cuanto)
    {
        ScoreManager.singleton.AumentarBonus(cuanto);
    }
}

[System.Serializable]
public class CartaPoder
{
    public string nombre;
    public Material material;
    public UnityEvent eventoActivar;
}
