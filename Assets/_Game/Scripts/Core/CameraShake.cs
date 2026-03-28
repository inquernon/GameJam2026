using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Camera Reference")]
    [SerializeField] private Camera targetCamera;

    [Header("Preset Settings")]
    [SerializeField] private ShakePreset shakeCorto = new ShakePreset(0.1f, 0.3f, 20f);
    [SerializeField] private ShakePreset shakeIntermedio = new ShakePreset(0.3f, 0.5f, 15f);
    [SerializeField] private ShakePreset superShake = new ShakePreset(0.6f, 1.0f, 25f);

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Coroutine currentShakeCoroutine;
    private bool isShaking = false;

    [System.Serializable]
    public class ShakePreset
    {
        public float duration;
        public float amplitude;
        public float frequency;

        public ShakePreset(float duration, float amplitude, float frequency)
        {
            this.duration = duration;
            this.amplitude = amplitude;
            this.frequency = frequency;
        }
    }

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

        // Obtener la cámara si no está asignada
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                targetCamera = GetComponent<Camera>();
            }
        }

        if (targetCamera != null)
        {
            SaveOriginalTransform();
        }
        else
        {
            Debug.LogError("CameraShake: No se encontró ninguna cámara!");
        }
    }

    private void SaveOriginalTransform()
    {
        originalPosition = targetCamera.transform.localPosition;
        originalRotation = targetCamera.transform.localRotation;
    }

    // Método principal para hacer shake personalizado
    public void Shake(float duration, float amplitude, float frequency)
    {
        if (targetCamera == null)
        {
            Debug.LogWarning("CameraShake: No hay cámara asignada!");
            return;
        }

        // Si ya hay un shake en curso, detenerlo
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
        }

        currentShakeCoroutine = StartCoroutine(ShakeCoroutine(duration, amplitude, frequency));
    }

    // Métodos prefabricados
    [ContextMenu("Shake Corto")]
    public void ShakeCorto()
    {
        Shake(shakeCorto.duration, shakeCorto.amplitude, shakeCorto.frequency);
        if (showDebugLogs)
            Debug.Log("CameraShake: Shake Corto activado");
    }

    [ContextMenu("Shake Intermedio")]
    public void ShakeIntermedio()
    {
        Shake(shakeIntermedio.duration, shakeIntermedio.amplitude, shakeIntermedio.frequency);
        if (showDebugLogs)
            Debug.Log("CameraShake: Shake Intermedio activado");
    }

    [ContextMenu("Super Shake")]
    public void SuperShake()
    {
        Shake(superShake.duration, superShake.amplitude, superShake.frequency);
        if (showDebugLogs)
            Debug.Log("CameraShake: Super Shake activado");
    }

    private IEnumerator ShakeCoroutine(float duration, float amplitude, float frequency)
    {
        isShaking = true;
        float elapsed = 0f;

        // Guardar la posición al inicio del shake por si la cámara se movió
        Vector3 startPosition = targetCamera.transform.localPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Calcular el porcentaje completado
            float percentComplete = elapsed / duration;

            // Reducir la amplitud con el tiempo (decay)
            float damper = 1f - Mathf.Clamp01(percentComplete);

            // Generar offset aleatorio usando Perlin Noise para un movimiento más suave
            float x = (Mathf.PerlinNoise(Time.time * frequency, 0f) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(0f, Time.time * frequency) - 0.5f) * 2f;
            float z = (Mathf.PerlinNoise(Time.time * frequency, Time.time * frequency) - 0.5f) * 2f;

            Vector3 offset = new Vector3(x, y, z) * amplitude * damper;

            // Aplicar el offset
            targetCamera.transform.localPosition = originalPosition + offset;

            yield return null;
        }

        // Volver a la posición original suavemente
        float returnDuration = 0.1f;
        float returnElapsed = 0f;
        Vector3 currentPosition = targetCamera.transform.localPosition;

        while (returnElapsed < returnDuration)
        {
            returnElapsed += Time.deltaTime;
            float t = returnElapsed / returnDuration;
            targetCamera.transform.localPosition = Vector3.Lerp(currentPosition, originalPosition, t);
            yield return null;
        }

        // Asegurar que vuelve exactamente a la posición original
        targetCamera.transform.localPosition = originalPosition;
        targetCamera.transform.localRotation = originalRotation;

        isShaking = false;
        currentShakeCoroutine = null;
    }

    // Método para detener el shake inmediatamente
    public void StopShake()
    {
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
            currentShakeCoroutine = null;
        }

        if (targetCamera != null)
        {
            targetCamera.transform.localPosition = originalPosition;
            targetCamera.transform.localRotation = originalRotation;
        }

        isShaking = false;

        if (showDebugLogs)
            Debug.Log("CameraShake: Shake detenido");
    }

    // Método para verificar si está haciendo shake
    public bool IsShaking()
    {
        return isShaking;
    }

    // Método para actualizar la cámara objetivo
    public void SetTargetCamera(Camera newCamera)
    {
        targetCamera = newCamera;
        if (targetCamera != null)
        {
            SaveOriginalTransform();
        }
    }

    // Método para actualizar los presets en runtime
    public void UpdateShakeCortoPreset(float duration, float amplitude, float frequency)
    {
        shakeCorto = new ShakePreset(duration, amplitude, frequency);
    }

    public void UpdateShakeIntermedioPreset(float duration, float amplitude, float frequency)
    {
        shakeIntermedio = new ShakePreset(duration, amplitude, frequency);
    }

    public void UpdateSuperShakePreset(float duration, float amplitude, float frequency)
    {
        superShake = new ShakePreset(duration, amplitude, frequency);
    }

    // Método para obtener la cámara actual
    public Camera GetTargetCamera()
    {
        return targetCamera;
    }

    // Resetear la posición original (útil si la cámara cambia de posición)
    public void ResetOriginalPosition()
    {
        if (targetCamera != null)
        {
            SaveOriginalTransform();
            if (showDebugLogs)
                Debug.Log("CameraShake: Posición original actualizada");
        }
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