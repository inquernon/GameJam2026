using UnityEngine;

public class Nodo : MonoBehaviour
{
    public GameObject[] objetos;
    [Range(0f, 1f)]
    public float probabilidad = 0.5f;
    public GameObject[] carros;
    [Range(0f, 1f)]
    public float probabilidadCarros = 0.2f;
    public Transform pivote;
    [Range(0f, 1f)]
    public float probabilidadZombies = 0.2f;
    public GameObject zombiePrefab;
    public Transform[] puntosZombies;

    private void Start()
    {
        Resetear();
    }
    public void Resetear()
    {
        for (int i = 0; i < objetos.Length; i++)
        {
            objetos[i].SetActive(Random.Range(0f, 1f) < probabilidad);
            objetos[i].transform.Rotate(Vector3.forward * Random.Range(0, 350));
        }
        for (int i = 0; i < carros.Length; i++)
        {
            carros[i].SetActive(Random.Range(0f, 1f) < probabilidadCarros);
        }
        int r = 0;
        for (int i = 0; i < puntosZombies.Length; i++)
        {
            if(Random.Range(0f, 1f) < probabilidadZombies)
            {
                Instantiate(zombiePrefab, puntosZombies[i].position, Quaternion.Euler(0,180,0));
            }
        }
    }
    private void Update()
    {
    }
}
