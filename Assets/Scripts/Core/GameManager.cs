using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    private int score=0;
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
        //editar UI
    }
    public void OnPlayerDeath()
    {
        Debug.Log($"Juego finalizado {score}");
    }

}
