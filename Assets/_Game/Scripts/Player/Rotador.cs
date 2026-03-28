using UnityEngine;

public class Rotador : MonoBehaviour
{
    public Vector3 velocidad;
    public float velo;
    public Transform referencia;
    void Update()
    {
        transform.RotateAround(referencia.position, velocidad, velo* Time.deltaTime);
    }
}
