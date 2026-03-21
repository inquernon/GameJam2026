using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MotoController : MonoBehaviour
{
    [Header("velocidad")]
    [Tooltip("Velocidad inicial")]
    [SerializeField] private float velocidadInicial = 5f;

    [Tooltip("Aumento de velocidad por segundo")]
    [SerializeField] private float aceleracion = 0.05f;

    [Tooltip("Velocidad maxima alcanzable")]
    [SerializeField] private float velocidadMaxima = 30f;

    [Header("Movimiento lateral")]
    [Tooltip("Velocidad lado a lado")]
    [SerializeField] private float velocidadLateral = 5f;
    [SerializeField] private float suavizadoLateral = 8f;

    [Header("Giroscopio")]
    [Tooltip("Sensibilidad del giroscopio")]
    [SerializeField] private float sensibilidadGiroscopio = 2f;

    [Tooltip("Angulo neutro del telefono (calibracion)")]
    [SerializeField] private float anguloNeutro=0;
    [SerializeField] private MotoAnimator motoAnimator;

    private Rigidbody rb;
    private float velocidadActual;
    private float inputLateral; //valor suavizado
    private float inputLateralRaw; //valor real recibido
    private bool estaMuerto = false;
    private bool usarGiroscopio = false;

    public float CurrentSpeed => velocidadActual;
    public bool EstaFrenando {get; private set;}

    public void SetLateralInput(float value)
    {
        inputLateralRaw = Mathf.Clamp(value, -1f,1f);
    }

    //metodo para reducir la velocidad
    public void ReduceSpeed(float cant)
    {
        velocidadActual = Mathf.Max(velocidadInicial,velocidadActual-cant);
    }
    
    public void TriggerDeath()
    {
        if(estaMuerto) return;

        estaMuerto = true;
        rb.isKinematic = false;
        motoAnimator?.PlayDeath();
        //vuelo y caida
        //rb.AddForce(transform.forward * velocidadActual + Vector3.up*4f, ForceMode.Impulse);
        //rb.AddTorque(Random.insideUnitSphere *3f, ForceMode.Impulse);
        
        GameManager.Instance?.OnPlayerDeath();
    }

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        velocidadActual = velocidadInicial;
        motoAnimator = GetComponentInChildren<MotoAnimator>();
    }

    private void Start()
    {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        if(SystemInfo.supportsGyroscope){
            Input.gyro.enabled = true;
            usarGiroscopio = true;
        }
#endif
    }

    private void Update()
    {
        if(estaMuerto) return;
    // Input de teclado para probar en editor (A/D o flechas)
        SetLateralInput(Input.GetAxis("Horizontal"));
        ReadInputGyroscope();
        InterpolateInput();

        if (Input.GetKeyDown(KeyCode.Space)) TriggerDeath();

//luego hay que agregar el giroscopio tambien para frenar
        EstaFrenando = Input.GetKey(KeyCode.S);

    }

    
    private void FixedUpdate()
    {
        if(estaMuerto) return;

        Accelerate();
        ApplyMovement();
    }

    //Movimiento del personaje
    private void Accelerate()
    {
        velocidadActual = Mathf.Min(velocidadActual + aceleracion * Time.fixedDeltaTime, velocidadMaxima);
    }

    private void ApplyMovement()
    {
        Vector3 movimiento = new Vector3(inputLateral*velocidadLateral,0f,velocidadActual);
        rb.MovePosition(rb.position + movimiento * Time.fixedDeltaTime);
    }

    private void InterpolateInput()
    {
        inputLateral = Mathf.Lerp(inputLateral, inputLateralRaw,suavizadoLateral*Time.deltaTime); 
    }

    //giroscopio
    private void ReadInputGyroscope()
    {
        if(!usarGiroscopio) return;

        //gravity.x para tener la inclinacion lateral del telefono
        float inclinacion = Input.gyro.gravity.x - anguloNeutro;
        SetLateralInput(inclinacion*sensibilidadGiroscopio);
    }

    //calibracion del angulo nuetro con la posicion actual del telefono
    //llamar al inicio del juego o con un boton de calibrar
    public void CalibrateGyroscope()
    {
        if(usarGiroscopio) anguloNeutro = Input.gyro.gravity.x;
    }
}
