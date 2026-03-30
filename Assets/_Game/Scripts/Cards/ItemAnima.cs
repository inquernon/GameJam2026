using UnityEngine;

public class ItemAnima : MonoBehaviour
{
    public Transform tItem;
    public float amplitud, frecuencia, velAngular;

    void FixedUpdate()
    {
        tItem.localPosition = Vector3.up * amplitud*Mathf.Sin(frecuencia*Time.time);
        tItem.localEulerAngles = velAngular*Time.time*Vector3.up;
    }
}
