
using UnityEngine;
[RequireComponent(typeof(MotoController))]
public class MotoCollisioner : MonoBehaviour
{
    [SerializeField] private float reduccionVelocidadZombie = 2f;

    [SerializeField] private string tagZombie;
    [SerializeField] private string tagObstaculo;

    private MotoController motoController;

    private void Awake()
    {
        motoController = GetComponent<MotoController>();    
    }

    //trigger con los que atropella
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(tagObstaculo)){
            motoController.TriggerDeath();
        }
    }

}
