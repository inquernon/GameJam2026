using UnityEngine;
using TMPro;
public class CargaScore : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMeshProUGUI.text = "Record: " + PlayerPrefs.GetInt("score", 0).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
