using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MotoAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayDeath()
    {
        animator.SetBool("estaMuerto", true);
    }
    public void PlayIdle()
    {
        animator.SetBool("estaMuerto",false);
    }

    public void PlayQuieto()
    {
        animator.Play("Esqueleto|Quieto_001");
    }
}
