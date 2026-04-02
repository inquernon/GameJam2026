using UnityEngine;

public class DificultadProgresiva : MonoBehaviour
{
    [SerializeField] private float incrementoVelocidad = 0.1f; // Incremento de velocidad por distancia recorrida
    [SerializeField] private float distanciaParaIncremento = 100f; // Distancia que el jugador debe recorrer para que aumente la velocidad
    [SerializeField] private MotoController motoController;
    private float distanciaRecorrida = 0f;
    private float umbral;
    public void Start()
    {
        umbral = distanciaParaIncremento;
    }
    private void Update()
    {
        if (motoController.estaMuerto) return;
        // Calcular la distancia recorrida
        distanciaRecorrida = motoController.transform.position.z;
        // Verificar si se ha alcanzado la distancia para incrementar la velocidad
        if (distanciaRecorrida >= umbral)
        {
            motoController.IncreaseDifficulty(incrementoVelocidad);
            umbral += distanciaParaIncremento;
        }
    }

}
