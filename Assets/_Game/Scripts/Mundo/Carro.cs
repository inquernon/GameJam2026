using UnityEngine;

public class Carro : MonoBehaviour
{
    public GameObject[] listaCarros;

    private void Start()
    {
        int cual = Random.Range(0, listaCarros.Length);
        for (int i = 0; i < listaCarros.Length; i++)
        {
            listaCarros[i].SetActive(i == cual);
        }
    }
}
