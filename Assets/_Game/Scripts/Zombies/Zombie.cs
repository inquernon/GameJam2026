using UnityEngine;

public class Zombie : MonoBehaviour
{
    public RagdollController ragdollController;
    public GameObject sangre;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Morir(); 
        }
    }

    public void Morir()
    {
        ragdollController.ActivarMuerte();
        sangre.SetActive(true);
    }
}
