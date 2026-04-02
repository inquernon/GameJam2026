using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CuentaRegresiva : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoCuenta;
    [SerializeField] private MotoController motoController;
    [SerializeField] private MotoAnimator motoAnimator;
    private void Start()
    {
        motoController.enabled = false;
        motoAnimator.PlayQuieto();
        StartCoroutine(IniciarCuenta());
    }

    private IEnumerator IniciarCuenta()
    {
        textoCuenta.text = "3";
        yield return new WaitForSeconds(1f);

        textoCuenta.text = "2";
        yield return new WaitForSeconds(1f);

        textoCuenta.text = "1";
        yield return new WaitForSeconds(1f);

        textoCuenta.text = "¡YA!";
        yield return new WaitForSeconds(0.5f);

        textoCuenta.gameObject.SetActive(false);
        
        motoAnimator.PlayIdle();
        GameManager.Instance?.IniciarJuego();
        motoController.enabled = true;
    }
}