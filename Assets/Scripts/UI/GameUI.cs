using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoScore;
    [SerializeField] private Slider sliderGasolina;
    [SerializeField] private FuelSystem fuelSystem;

    private void Update()
    {
        sliderGasolina.value = fuelSystem.CombustibleNormalizado;
    }

    public void ActualizarScore(int score)
    {
        textoScore.text = "Score: " + score;
    }
}