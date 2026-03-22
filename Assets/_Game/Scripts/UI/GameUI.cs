using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoScore;
   
    public void ActualizarScore(int score)
    {
        textoScore.text = "Score: " + score;
    }
}