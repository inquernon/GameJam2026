using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform objetivo;
    [SerializeField] private Vector3 offset = new Vector3(0f,8f,-6f);

    private void LateUpdate()
    {
        if(objetivo==null) return;
        transform.position = objetivo.position + offset;
        transform.LookAt(objetivo);
    }
}
