using System.Collections;
using UnityEngine;

public class FuelSystem : MonoBehaviour
{
    [SerializeField] private float combustibleMaximo = 100f;
    [SerializeField] private float consumoPorSegundo = 5f;
    [SerializeField] private float consumoExtra = 10f; // cuando se frena o choca con un obstaculo

    private float combustibleActual;
    private MotoController motoController;

    public float CombustibleNormalizado => combustibleActual / combustibleMaximo;

    private void Awake()
    {
        combustibleActual = combustibleMaximo;
        motoController = GetComponent<MotoController>();
    }
    private void Update()
    {
        if(combustibleActual <= 0) return;
        if (GameManager.Instance.EstaJugando == false) return;
        ConsumirCombustible();
        if(combustibleActual <= 0)
            GameManager.Instance?.OnPlayerDeath();
    }
    private void ConsumirCombustible()
    {
        float consumo = consumoPorSegundo;
        if (motoController.EstaFrenando)
        {
            consumo += consumoExtra;
        }

        combustibleActual -= consumo * Time.deltaTime;
        combustibleActual = Mathf.Max(combustibleActual,0f);
    }
    public void RecargarCombustible(float cant)
    {
        combustibleActual = Mathf.Min(combustibleActual+ cant, combustibleMaximo);
    }

    public void ImpactoZombie(float gasolina, int puntos)
    {
        RecargarCombustible(gasolina);
        GameManager.Instance?.AddScore(puntos);
    }

    public void ImpactoObstaculo(float gasolinaQueQuita)
    {
        combustibleActual = Mathf.Max(combustibleActual - gasolinaQueQuita, 0f);
    }

}
