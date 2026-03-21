using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodoControl : MonoBehaviour
{
    public static NodoControl singleton;
    public GameObject[] elementosBase;
    public int cuantosBase = 10;
    public List<Nodo> nodos;
    public List<Nodo> nodosActivos;
    public int numeroNodosActivos = 3;

    public Transform jugador;
    public int indice = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nodos = new List<Nodo>();
        nodosActivos = new List<Nodo>();
        for (int i = 0; i < cuantosBase; i++)
        {
            GameObject n = Instantiate(elementosBase[Random.Range(0, elementosBase.Length)], transform);
            nodos.Add(n.GetComponent<Nodo>());
            n.SetActive(false);
        }
        for (int i = 0; i < numeroNodosActivos; i++)
        {
            int cual = Random.Range(0,nodos.Count);
            if(nodosActivos.Count == 0)
            {
                nodos[cual].transform.position = Vector3.zero;
            }
            else
            {
                nodos[cual].transform.position = nodosActivos[nodosActivos.Count - 1].pivote.position;
            }
            nodosActivos.Add(nodos[cual]);
            nodos[cual].gameObject.SetActive(true);
            nodos.RemoveAt(cual);
        }
    }

    public void CrearNodo()
    {
        int cual = Random.Range(0, nodos.Count);
        nodos[cual].transform.position = nodosActivos[nodosActivos.Count - 1].pivote.position;
        nodos[cual].gameObject.SetActive(true);
        nodosActivos.Add(nodos[cual]);
        nodos.RemoveAt(cual);

        nodos.Add(nodosActivos[0]);
        nodosActivos[0].gameObject.SetActive(false);
        nodosActivos.RemoveAt(0);


    }
    private void FixedUpdate()
    {
        if (jugador.position.z > 50*indice+25)
        {
            CrearNodo();
            indice++;
        }
    }

}
