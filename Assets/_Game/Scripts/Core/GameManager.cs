using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    
    [SerializeField] private GameUI gameUI;    
    [SerializeField] private PantallaGameOver pantallaGameOver;
    public AudioSource sonidoChoque;
    private int scoreFinal = 0;
    public bool EstaJugando {  get; private set; } = false;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        ScreenFader.Instance?.FadeIn();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    public void IniciarJuego()
    {
        EstaJugando = true;
    }
    public void OnPlayerDeath()
    {
        EstaJugando = false;
        sonidoChoque.Play();
        StartCoroutine(MostrarGameOverConDelay());
        CameraShake.Instance.ShakeIntermedio();
        VibrationManager.singleton.VibrarAlto();
    }

    private IEnumerator MostrarGameOverConDelay()
    {
        yield return new WaitForSeconds(1f);
        pantallaGameOver?.Mostrar((int)scoreFinal);
    }

}
