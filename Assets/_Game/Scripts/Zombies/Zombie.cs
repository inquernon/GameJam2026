using UnityEngine;

public class Zombie : MonoBehaviour
{
    public RagdollController ragdollController;
    public GameObject sangre;
    public float desaceleracion=1;
    public GameObject[] objetosDesactivables;
    public AudioSource audio;
    private void Start()
    {
        Destroy(gameObject,40);
        for (int i = 0; i < objetosDesactivables.Length; i++)
        {
            objetosDesactivables[i].SetActive(Random.Range(0f, 1f) < 0.7f);
        }
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
        audio.Play();
        MotoController.singleton.ReduceSpeed(desaceleracion);
    }
}
