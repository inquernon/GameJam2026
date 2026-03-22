using UnityEngine;
using UnityEngine.EventSystems;

public class ControlesWebGL : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private MotoController motoController;
    [SerializeField] private float valorInput = 1f; // 1 para derecha, -1 para izquierda

    private bool presionado = false;
    private void Awake()
    {
#if !UNITY_WEBGL
        gameObject.SetActive(false);
#endif
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        presionado = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        presionado = false;
        motoController.SetLateralInput(0f);
    }

    private void Update()
    {
        if (presionado)
            motoController.SetLateralInput(valorInput);
    }
}