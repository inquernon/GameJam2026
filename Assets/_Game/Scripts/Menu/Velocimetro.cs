using UnityEngine;
using UnityEngine.Audio;

public class Velocimetro : MonoBehaviour
{
    public Vector2 angulos;
    float t;
    public AudioSource audioSource;
    public Vector2 pitch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t = (MotoController.singleton.velocidadActual - MotoController.singleton.VelocidadInicial) / (MotoController.singleton.VelocidadMaxima- MotoController.singleton.VelocidadInicial);
        transform.localEulerAngles = Vector3.forward * Mathf.Lerp(angulos.x,angulos.y,t);
        if(audioSource != null )
        {
            if (MotoController.singleton.estaMuerto)
            {
                audioSource.Stop();
                enabled = false;
            }
            audioSource.pitch = Mathf.Lerp(pitch.x,pitch.y,t);
        }
    }
}
