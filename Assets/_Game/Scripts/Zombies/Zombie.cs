using UnityEngine;

public class Zombie : MonoBehaviour
{
    public RagdollController ragdollController;
    public GameObject sangre;
    public float desaceleracion=1;
    private void Start()
    {
        Destroy(gameObject,40);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Morir();
            Debug.LogWarning("MUERTO POR CAUSA DE: " + other.gameObject.name);
        }
    }

    public void Morir()
    {
        ragdollController.ActivarMuerte();
        sangre.SetActive(true);
        MotoController.singleton.ReduceSpeed(desaceleracion);
    }
}
