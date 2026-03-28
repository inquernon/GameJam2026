using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MotoController : MonoBehaviour
{
    public static MotoController singleton;
    [Header("velocidad")]
    [Tooltip("Velocidad inicial")]
    [SerializeField] public float velocidadInicial = 5f;

    [Tooltip("Aumento de velocidad por segundo")]
    [SerializeField] private float aceleracion = 0.05f;

    [Tooltip("Velocidad maxima alcanzable")]
    [SerializeField] public float velocidadMaxima = 30f;

    [Header("Acelerometro")]
    [Tooltip("Sensibilidad del acelerometro")]
    [SerializeField] private float sensibilidadAcelerometro = 2f;

    [Tooltip("Angulo neutro del telefono (calibracion)")]
    [SerializeField] private float anguloNeutro = 0;

    [SerializeField] private MotoAnimator motoAnimator;

    private Rigidbody rb;
    [SerializeField] public float velocidadActual;
    private float anguloObjetivo;
    [SerializeField] private float suavizadoRotacion = 5f;
    [SerializeField] private float anguloMaximo = 45f;
    private float inputLateralRaw; //valor real recibido
    [SerializeField] private bool estaMuerto = false;
    private bool usarAcelerometro = false;

    public float CurrentSpeed => velocidadActual;
    public bool EstaFrenando { get; private set; }

    public GameObject particulasMuerte;

    public void SetLateralInput(float value)
    {
        inputLateralRaw = Mathf.Clamp(value, -1f, 1f);
    }

    //metodo para reducir la velocidad
    public void ReduceSpeed(float cant)
    {
        velocidadActual = Mathf.Max(velocidadInicial, velocidadActual - cant);
    }

    public void TriggerDeath()
    {
        if (estaMuerto) return;
        particulasMuerte.SetActive(true);
        estaMuerto = true;
        rb.linearVelocity = Vector3.zero;
        motoAnimator?.PlayDeath();

        GameManager.Instance?.OnPlayerDeath();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        velocidadActual = velocidadInicial;
        motoAnimator = GetComponentInChildren<MotoAnimator>();
        singleton = this;
    }

    private void Start()
    {
        // Verificar si el dispositivo soporta acelerometro
        if (SystemInfo.supportsAccelerometer)
        {
            usarAcelerometro = true;
            //Debug.Log("Acelerometro activado");
        }
        else
        {
            //Debug.Log("Acelerometro no disponible, usando teclado");
        }
    }

    private void Update()
    {
        if (estaMuerto) return;

        // Input de teclado para probar en editor (A/D o flechas)
        SetLateralInput(Input.GetAxis("Horizontal"));

        // Leer acelerometro
        ReadInputAccelerometer();

        InterpolateInput();

        // Freno
        EstaFrenando = Input.GetKey(KeyCode.S) || LeerFrenoAcelerometro();
    }

    private void FixedUpdate()
    {
        if (estaMuerto)
        {
            return;
        }
        else
        {
            Accelerate();
            ApplyMovement();
            ApplyRotation();
        }
    }

    //Movimiento del personaje
    private void Accelerate()
    {
        velocidadActual = Mathf.Min(velocidadActual + aceleracion * Time.fixedDeltaTime, velocidadMaxima);
    }

    private void ApplyMovement()
    {
        if (estaMuerto) return;
        Vector3 direccion = rb.rotation * Vector3.forward;
        rb.MovePosition(rb.position + direccion * velocidadActual * Time.fixedDeltaTime);
    }

    private void ApplyRotation()
    {
        if (estaMuerto) return;
        Quaternion rotacionObjetivo = Quaternion.Euler(0f, anguloObjetivo, 0f);
        rb.rotation = Quaternion.Lerp(rb.rotation, rotacionObjetivo, suavizadoRotacion * velocidadActual * Time.fixedDeltaTime);
    }

    private void InterpolateInput()
    {
        anguloObjetivo = inputLateralRaw * anguloMaximo;
    }

    //==================== ACELEROMETRO ====================
    private void ReadInputAccelerometer()
    {
        if (!usarAcelerometro) return;

        // Input.acceleration devuelve la aceleracion en los 3 ejes
        // .x es la inclinacion lateral
        Vector3 accel = Input.acceleration;

        float inclinacion = accel.x - anguloNeutro;
        SetLateralInput(inclinacion * sensibilidadAcelerometro);
    }

    private bool LeerFrenoAcelerometro()
    {
        if (!usarAcelerometro) return false;

        // Si el telefono apunta hacia arriba (acceleration.y positivo)
        return Input.acceleration.y > 0.5f;
    }

    //==================== CALIBRACION ====================
    // Calibracion del angulo neutro con la posicion actual del telefono
    // Llamar al inicio del juego o con un boton de calibrar
    public void CalibrateAccelerometer()
    {
        if (usarAcelerometro)
        {
            anguloNeutro = Input.acceleration.x;
        }
    }

}