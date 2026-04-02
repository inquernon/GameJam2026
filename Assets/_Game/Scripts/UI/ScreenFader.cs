using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Initial State")]
    [SerializeField] private bool startFadedOut = false; // Empezar con pantalla negra

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private Coroutine currentFadeCoroutine;
    private bool isFading = false;

    private void Awake()
    {
        // Implementación del Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Validar que hay una imagen asignada
        if (fadeImage == null)
        {
            fadeImage = GetComponent<Image>();
            if (fadeImage == null)
            {
                Debug.LogError("ScreenFader: No se encontró una Image asignada!");
                return;
            }
        }

        // Asegurar que el Canvas está configurado correctamente
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = 9999; // Asegurar que esté al frente
        }

        // Configurar estado inicial
        if (startFadedOut)
        {
            SetAlpha(1f);

        }
        else
        {
            SetAlpha(0f);
        }
    }

    // FadeOut: Pantalla se pone negra (alpha 0 -> 1)
    [ContextMenu("Fade Out")]
    public void FadeOut()
    {
        FadeOut(fadeDuration);
    }

    public void FadeOut(float duration)
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("ScreenFader: No hay imagen asignada!");
            return;
        }

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeCoroutine(fadeImage.color.a, 1f, duration));

        if (showDebugLogs)
            Debug.Log("ScreenFader: FadeOut iniciado");
    }

    // FadeIn: Pantalla se vuelve transparente (alpha 1 -> 0)
    [ContextMenu("Fade In")]
    public void FadeIn()
    {
        FadeIn(fadeDuration);
    }

    public void FadeIn(float duration)
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("ScreenFader: No hay imagen asignada!");
            return;
        }

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeCoroutine(fadeImage.color.a, 0f, duration));

        if (showDebugLogs)
            Debug.Log("ScreenFader: FadeIn iniciado");
    }

    private IEnumerator FadeCoroutine(float startAlpha, float targetAlpha, float duration)
    {
        isFading = true;
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = fadeCurve.Evaluate(t);

            color.a = Mathf.Lerp(startAlpha, targetAlpha, curveValue);
            fadeImage.color = color;

            yield return null;
        }

        // Asegurar que termina en el valor exacto
        color.a = targetAlpha;
        fadeImage.color = color;

        isFading = false;
        currentFadeCoroutine = null;

        if (showDebugLogs)
            Debug.Log($"ScreenFader: Fade completado (alpha: {targetAlpha})");
    }

    // Método para establecer el alpha inmediatamente sin animación
    public void SetAlpha(float alpha)
    {
        if (fadeImage == null) return;

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = null;
        }

        Color color = fadeImage.color;
        color.a = Mathf.Clamp01(alpha);
        fadeImage.color = color;
        isFading = false;
    }

    // Método para verificar si está haciendo fade
    public bool IsFading()
    {
        return isFading;
    }

    // Método para obtener el alpha actual
    public float GetCurrentAlpha()
    {
        if (fadeImage == null) return 0f;
        return fadeImage.color.a;
    }

    // Método para cambiar la duración del fade
    public void SetFadeDuration(float duration)
    {
        fadeDuration = Mathf.Max(0.1f, duration);
    }

    // Método para cambiar la imagen de fade
    public void SetFadeImage(Image newImage)
    {
        fadeImage = newImage;
    }

    // Método para cambiar el color del fade (útil para transiciones de colores)
    public void SetFadeColor(Color color)
    {
        if (fadeImage == null) return;

        Color currentColor = fadeImage.color;
        color.a = currentColor.a; // Mantener el alpha actual
        fadeImage.color = color;
    }

    // Métodos útiles para transiciones de escena
    public void FadeOutAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoadSceneCoroutine(sceneName, fadeDuration));
    }

    public void FadeOutAndLoadScene(string sceneName, float duration)
    {
        StartCoroutine(FadeOutAndLoadSceneCoroutine(sceneName, duration));
    }

    private IEnumerator FadeOutAndLoadSceneCoroutine(string sceneName, float duration)
    {
        FadeOut(duration);

        // Esperar a que termine el fade
        while (isFading)
        {
            yield return null;
        }

        // Cargar la escena
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void FadeOutAndLoadScene(int sceneIndex)
    {
        StartCoroutine(FadeOutAndLoadSceneCoroutine(sceneIndex, fadeDuration));
    }

    private IEnumerator FadeOutAndLoadSceneCoroutine(int sceneIndex, float duration)
    {
        FadeOut(duration);

        // Esperar a que termine el fade
        while (isFading)
        {
            yield return null;
        }

        // Cargar la escena
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    // Contexto de menú adicional para testing
    [ContextMenu("Fade Out Rápido")]
    private void FadeOutRapido()
    {
        FadeOut(0.3f);
    }

    [ContextMenu("Fade In Rápido")]
    private void FadeInRapido()
    {
        FadeIn(0.3f);
    }

    [ContextMenu("Set Alpha 0")]
    private void SetAlpha0()
    {
        SetAlpha(0f);
    }

    [ContextMenu("Set Alpha 1")]
    private void SetAlpha1()
    {
        SetAlpha(1f);
    }

    private void OnDestroy()
    {
        // Limpiar el singleton
        if (Instance == this)
        {
            Instance = null;
        }
    }
}