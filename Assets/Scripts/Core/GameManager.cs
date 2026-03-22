using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    [SerializeField] private GameUI gameUI;
    private int score=0; 
    [SerializeField] private PantallaGameOver pantallaGameOver;
    public bool EstaJugando {  get; private set; } = false;
    public void IniciarJuego()
    {
        EstaJugando = true;
    }
    private void Awake() {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
    }
    public void AddScore(int puntos)
    {
        score += puntos;
        Debug.Log($"Puntaje: {score}");
        gameUI?.ActualizarScore(score);
    }
    public void OnPlayerDeath()
    {
        EstaJugando = false;
        Debug.Log($"Juego finalizado {score}");
        pantallaGameOver?.Mostrar(score);
    }

}
