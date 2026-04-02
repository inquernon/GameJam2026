using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public static GameUI singleton;
    [SerializeField] private Text textoScore;
    [SerializeField] private Text textoZombies;

    private void Awake()
    {
        singleton = this;
    }

    public void ActualizarScore(int score)
    {
        textoScore.text = "Score: " + score;
    }
    public void ActualizarZombies(int score, int max)
    {
        textoZombies.text = score.ToString() + "/" + max.ToString();
    }
}