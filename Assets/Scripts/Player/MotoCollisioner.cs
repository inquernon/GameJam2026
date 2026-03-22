
using UnityEngine;
[RequireComponent(typeof(MotoController))]
[RequireComponent(typeof(FuelSystem))]
public class MotoCollisioner : MonoBehaviour
{
    [SerializeField] private float gasolinaZombie = 10f;
    [SerializeField] private int puntosZombie = 100;
    [SerializeField] private float reduccionVelocidadZombie = 2f;

    [SerializeField] private float gasolinaObstaculo = 15f;
    [SerializeField] private float reduccionVelocidadObstaculo = 5f;
    [SerializeField] private string tagColisionbuena;
    [SerializeField] private string tagColisionMala;
    private MotoController motoController;
    private FuelSystem fuelSystem;

    private void Awake()
    {
        motoController = GetComponent<MotoController>();
        fuelSystem = GetComponent<FuelSystem>();    
    }

    //triguer con los que atropella
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagColisionMala))
        {
            fuelSystem.ImpactoObstaculo(gasolinaObstaculo);
            motoController.ReduceSpeed(reduccionVelocidadObstaculo);
            Debug.Log($"Choco mal perdio {reduccionVelocidadObstaculo} de velocidad y {gasolinaObstaculo} de gasolina");
        }
        else if (other.CompareTag(tagColisionbuena))
        {
            Debug.Log($"atropello bien y gano {puntosZombie} ademas que la gasolina aumento {gasolinaZombie}");
            fuelSystem.ImpactoZombie(gasolinaZombie, puntosZombie);
            motoController.ReduceSpeed(reduccionVelocidadZombie);
        } else if(other.gameObject.CompareTag("Muro")){
            Debug.Log("Se estrello contra muro");
            motoController.TriggerDeath();
        }
    }

}
