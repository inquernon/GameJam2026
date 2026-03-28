using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager singleton { get; private set; }

    [Header("Vibration Settings")]
    [SerializeField] private bool vibracionActiva = true;

    [Header("Presets (en milisegundos)")]
    [SerializeField] private long duracionPoco = 50;
    [SerializeField] private long duracionMedio = 100;
    [SerializeField] private long duracionAlto = 200;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject vibrator;
#endif

    private void Awake()
    {
        // Singleton
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        singleton = this;
        DontDestroyOnLoad(gameObject);

        InicializarVibrador();
    }

    private void InicializarVibrador()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            // Obtener el contexto de Unity
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            
            // Obtener el servicio de vibración
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            vibrator = context.Call<AndroidJavaObject>("getSystemService", "vibrator");
            
            if (showDebugLogs)
                Debug.Log("VibrationManager: Vibrador inicializado correctamente");
        }
        catch (System.Exception e)
        {
            Debug.LogError("VibrationManager: Error al inicializar vibrador - " + e.Message);
        }
#else
        if (showDebugLogs)
            Debug.Log("VibrationManager: No está en Android, vibración simulada");
#endif
    }

    // Método principal para vibrar con duración personalizada
    public void Vibrar(long duracionMilisegundos)
    {
        if (!vibracionActiva)
        {
            if (showDebugLogs)
                Debug.Log("VibrationManager: Vibración desactivada");
            return;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            if (vibrator != null)
            {
                vibrator.Call("vibrate", duracionMilisegundos);
                
                if (showDebugLogs)
                    Debug.Log($"VibrationManager: Vibrando por {duracionMilisegundos}ms");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("VibrationManager: Error al vibrar - " + e.Message);
        }
#else
        // Usar la vibración simple de Unity como fallback (solo funciona en móviles)
        Handheld.Vibrate();

        if (showDebugLogs)
            Debug.Log($"VibrationManager: Vibración simulada ({duracionMilisegundos}ms)");
#endif
    }

    // Métodos predefinidos
    [ContextMenu("Vibrar Poco")]
    public void VibrarPoco()
    {
        Vibrar(duracionPoco);
    }

    [ContextMenu("Vibrar Medio")]
    public void VibrarMedio()
    {
        Vibrar(duracionMedio);
    }

    [ContextMenu("Vibrar Alto")]
    public void VibrarAlto()
    {
        Vibrar(duracionAlto);
    }

    // Método para cancelar la vibración (Android 8.0+)
    public void CancelarVibracion()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            if (vibrator != null)
            {
                vibrator.Call("cancel");
                
                if (showDebugLogs)
                    Debug.Log("VibrationManager: Vibración cancelada");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("VibrationManager: Error al cancelar vibración - " + e.Message);
        }
#endif
    }

    // Método para activar/desactivar la vibración
    public void SetVibracionActiva(bool activa)
    {
        vibracionActiva = activa;

        if (showDebugLogs)
            Debug.Log($"VibrationManager: Vibración {(activa ? "activada" : "desactivada")}");
    }

    // Método para verificar si la vibración está activa
    public bool EstaVibracionActiva()
    {
        return vibracionActiva;
    }

    // Método para cambiar los presets en runtime
    public void SetDuracionPoco(long duracion)
    {
        duracionPoco = duracion;
    }

    public void SetDuracionMedio(long duracion)
    {
        duracionMedio = duracion;
    }

    public void SetDuracionAlto(long duracion)
    {
        duracionAlto = duracion;
    }

    // Método para obtener las duraciones actuales
    public long GetDuracionPoco() => duracionPoco;
    public long GetDuracionMedio() => duracionMedio;
    public long GetDuracionAlto() => duracionAlto;
}