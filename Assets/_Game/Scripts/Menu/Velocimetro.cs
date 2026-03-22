using UnityEngine;

public class Velocimetro : MonoBehaviour
{
    public Vector2 angulos;
    float t;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t = (MotoController.singleton.velocidadActual - MotoController.singleton.velocidadInicial) / (MotoController.singleton.velocidadMaxima- MotoController.singleton.velocidadInicial);
        transform.localEulerAngles = Vector3.forward * Mathf.Lerp(angulos.x,angulos.y,t);
    }
}
