using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIFadeAnimator : MonoBehaviour
{
    [System.Serializable]
    public class UIElement
    {
        public RectTransform rectTransform;
        public CanvasGroup canvasGroup;

        [Header("Animation Offsets")]
        public Vector2 startPositionOffset = new Vector2(0, -50); // Desde dónde empieza (relativo)

        [HideInInspector] public Vector2 originalPosition;
    }

    [Header("UI Elements")]
    [SerializeField] private UIElement[] uiElements;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.6f;
    [SerializeField] private float delayBetweenElements = 0.1f;
    [SerializeField] private bool animateOnStart = true;
    [SerializeField] private float initialDelay = 0.2f;

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Additional Effects")]
    [SerializeField] private bool animateScale = true;
    [SerializeField] private float startScale = 0.8f;
    [SerializeField] private bool overshootEffect = true;
    [SerializeField] private float overshootAmount = 1.1f;

    [Header("Fade Out Settings")]
    [SerializeField] private bool reverseOrderOnFadeOut = true;
    [SerializeField] private float fadeOutDuration = 0.4f;

    private bool isAnimating = false;
    private Coroutine currentAnimation;

    private void Awake()
    {
        SetupElements();
    }

    private void Start()
    {
        if (animateOnStart)
        {
            HideAllImmediate();
            Invoke(nameof(FadeIn), initialDelay);
        }
    }

    private void SetupElements()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            // Auto-obtener CanvasGroup si no está asignado
            if (uiElements[i].canvasGroup == null && uiElements[i].rectTransform != null)
            {
                uiElements[i].canvasGroup = uiElements[i].rectTransform.GetComponent<CanvasGroup>();
                if (uiElements[i].canvasGroup == null)
                {
                    uiElements[i].canvasGroup = uiElements[i].rectTransform.gameObject.AddComponent<CanvasGroup>();
                }
            }

            // Guardar posición original
            if (uiElements[i].rectTransform != null)
            {
                uiElements[i].originalPosition = uiElements[i].rectTransform.anchoredPosition;
            }
        }
    }

    [ContextMenu("Fade In")]
    public void FadeIn()
    {
        if (isAnimating)
        {
            StopCurrentAnimation();
        }

        currentAnimation = StartCoroutine(FadeInSequence());
    }

    [ContextMenu("Fade Out")]
    public void FadeOut()
    {
        if (isAnimating)
        {
            StopCurrentAnimation();
        }

        currentAnimation = StartCoroutine(FadeOutSequence());
    }

    [ContextMenu("Hide Immediate")]
    public void HideAllImmediate()
    {
        foreach (UIElement element in uiElements)
        {
            if (element.canvasGroup != null)
            {
                element.canvasGroup.alpha = 0;
            }

            if (element.rectTransform != null)
            {
                element.rectTransform.anchoredPosition = element.originalPosition + element.startPositionOffset;
                if (animateScale)
                {
                    element.rectTransform.localScale = Vector3.one * startScale;
                }
            }
        }
    }

    [ContextMenu("Show Immediate")]
    public void ShowAllImmediate()
    {
        foreach (UIElement element in uiElements)
        {
            if (element.canvasGroup != null)
            {
                element.canvasGroup.alpha = 1;
            }

            if (element.rectTransform != null)
            {
                element.rectTransform.anchoredPosition = element.originalPosition;
                element.rectTransform.localScale = Vector3.one;
            }
        }
    }

    private IEnumerator FadeInSequence()
    {
        isAnimating = true;

        // Preparar todos los elementos
        foreach (UIElement element in uiElements)
        {
            if (element.canvasGroup != null)
            {
                element.canvasGroup.alpha = 0;
            }

            if (element.rectTransform != null)
            {
                element.rectTransform.anchoredPosition = element.originalPosition + element.startPositionOffset;
                if (animateScale)
                {
                    element.rectTransform.localScale = Vector3.one * startScale;
                }
            }
        }

        // Animar cada elemento con delay
        for (int i = 0; i < uiElements.Length; i++)
        {
            StartCoroutine(AnimateElementIn(uiElements[i], i));
            yield return new WaitForSeconds(delayBetweenElements);
        }

        // Esperar a que termine la última animación
        yield return new WaitForSeconds(animationDuration);

        isAnimating = false;
    }

    private IEnumerator FadeOutSequence()
    {
        isAnimating = true;

        // Determinar el orden
        if (reverseOrderOnFadeOut)
        {
            // Animar en orden inverso
            for (int i = uiElements.Length - 1; i >= 0; i--)
            {
                StartCoroutine(AnimateElementOut(uiElements[i]));
                yield return new WaitForSeconds(delayBetweenElements);
            }
        }
        else
        {
            // Animar en orden normal
            for (int i = 0; i < uiElements.Length; i++)
            {
                StartCoroutine(AnimateElementOut(uiElements[i]));
                yield return new WaitForSeconds(delayBetweenElements);
            }
        }

        // Esperar a que termine la última animación
        yield return new WaitForSeconds(fadeOutDuration);

        isAnimating = false;
    }

    private IEnumerator AnimateElementIn(UIElement element, int index)
    {
        if (element.rectTransform == null || element.canvasGroup == null)
            yield break;

        float elapsed = 0f;
        Vector2 startPos = element.rectTransform.anchoredPosition;
        Vector2 targetPos = element.originalPosition;
        Vector3 startScaleVec = element.rectTransform.localScale;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            // Fade in
            float fadeValue = fadeCurve.Evaluate(t);
            element.canvasGroup.alpha = fadeValue;

            // Movimiento
            float moveValue = movementCurve.Evaluate(t);

            // Efecto overshoot en posición
            if (overshootEffect && t > 0.5f)
            {
                float overshootT = (t - 0.5f) * 2f; // 0 a 1 en la segunda mitad
                float overshoot = Mathf.Sin(overshootT * Mathf.PI) * 10f;
                element.rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, moveValue) + Vector2.up * overshoot;
            }
            else
            {
                element.rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, moveValue);
            }

            // Escala
            if (animateScale)
            {
                float scaleValue = scaleCurve.Evaluate(t);
                float currentScale = Mathf.Lerp(startScale, 1f, scaleValue);

                // Efecto overshoot en escala
                if (overshootEffect && t > 0.6f)
                {
                    float overshootT = (t - 0.6f) * 2.5f;
                    float scaleOvershoot = Mathf.Lerp(1f, overshootAmount, Mathf.Sin(overshootT * Mathf.PI));
                    currentScale *= scaleOvershoot;
                }

                element.rectTransform.localScale = Vector3.one * currentScale;
            }

            yield return null;
        }

        // Asegurar valores finales
        element.canvasGroup.alpha = 1f;
        element.rectTransform.anchoredPosition = targetPos;
        element.rectTransform.localScale = Vector3.one;
    }

    private IEnumerator AnimateElementOut(UIElement element)
    {
        if (element.rectTransform == null || element.canvasGroup == null)
            yield break;

        float elapsed = 0f;
        Vector2 startPos = element.rectTransform.anchoredPosition;
        Vector2 targetPos = element.originalPosition + element.startPositionOffset;
        Vector3 startScaleVec = element.rectTransform.localScale;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;

            // Fade out
            float fadeValue = fadeCurve.Evaluate(t);
            element.canvasGroup.alpha = 1f - fadeValue;

            // Movimiento
            float moveValue = movementCurve.Evaluate(t);
            element.rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, moveValue);

            // Escala
            if (animateScale)
            {
                float scaleValue = scaleCurve.Evaluate(t);
                float currentScale = Mathf.Lerp(1f, startScale, scaleValue);
                element.rectTransform.localScale = Vector3.one * currentScale;
            }

            yield return null;
        }

        // Asegurar valores finales
        element.canvasGroup.alpha = 0f;
        element.rectTransform.anchoredPosition = targetPos;
        if (animateScale)
        {
            element.rectTransform.localScale = Vector3.one * startScale;
        }
    }

    private void StopCurrentAnimation()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
        StopAllCoroutines();
        isAnimating = false;
    }

    // Métodos públicos adicionales
    public void SetAnimationDuration(float duration)
    {
        animationDuration = Mathf.Max(0.1f, duration);
    }

    public void SetDelayBetweenElements(float delay)
    {
        delayBetweenElements = Mathf.Max(0f, delay);
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }

    // Método para animar un elemento específico
    public void FadeInElement(int index)
    {
        if (index >= 0 && index < uiElements.Length)
        {
            StartCoroutine(AnimateElementIn(uiElements[index], index));
        }
    }

    public void FadeOutElement(int index)
    {
        if (index >= 0 && index < uiElements.Length)
        {
            StartCoroutine(AnimateElementOut(uiElements[index]));
        }
    }

    // Método para resetear todo
    [ContextMenu("Reset All")]
    public void ResetAll()
    {
        StopCurrentAnimation();
        SetupElements();
        HideAllImmediate();
    }
}