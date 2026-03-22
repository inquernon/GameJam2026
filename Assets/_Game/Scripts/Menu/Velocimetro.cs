using UnityEngine;

public class Velocimetro : MonoBehaviour
{
    public Vector2 angulos;
    float t;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //t = MotoController.singleton.maci
        transform.localEulerAngles = Vector3.forward;
    }
}
