using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMagnet : MonoBehaviour
{
    public static EnemyMagnet singleton { get; private set; }

    [Header("Magnet Settings")]
    [SerializeField] private float rangoAtraccion = 10f;
    [SerializeField] private float fuerzaAtraccion = 15f;
    [SerializeField] private float duracionIman = 5f;
    [SerializeField] private float distanciaLiberacion = 1f; // Distancia para liberar el enemigo

    [Header("Detection")]
    [SerializeField] private LayerMask capasDeteccion = -1; // Todas las capas por defecto
    [SerializeField] private float intervaloDeteccion = 0.2f; // Cada cuanto detecta enemigos

    [Header("Visual Effects")]
    [SerializeField] private GameObject efectoVisualIman;
    [SerializeField] private ParticleSystem particulasAtraccion;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoActivacion;
    [SerializeField] private AudioClip sonidoAtraccion; // Loop mientras esta activo
    [SerializeField] private AudioClip sonidoDesactivacion;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;
    [SerializeField] private Color gizmoColor = Color.cyan;

    private bool imanActivo = false;
    private float tiempoRestante = 0f;
    private List<GameObject> enemigosAtraidos = new List<GameObject>();
    private Coroutine deteccionCoroutine;
    private Coroutine duracionCoroutine;

    private void Awake()
    {
        // Singleton
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        singleton = this;

        // Auto-configurar AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        // Desactivar efectos visuales al inicio
        if (efectoVisualIman != null)
            efectoVisualIman.SetActive(false);

        if (particulasAtraccion != null)
            particulasAtraccion.Stop();
    }

    [ContextMenu("Activar Iman")]
    public void ActivarIman()
    {
        if (imanActivo)
        {
            if (showDebugLogs)
                Debug.LogWarning("EnemyMagnet: El iman ya esta activo");
            return;
        }

        imanActivo = true;
        tiempoRestante = duracionIman;

        // Limpiar lista de enemigos
        enemigosAtraidos.Clear();

        // Activar efectos visuales
        if (efectoVisualIman != null)
            efectoVisualIman.SetActive(true);

        if (particulasAtraccion != null)
            particulasAtraccion.Play();

        // Reproducir sonido de activacion
        if (sonidoActivacion != null)
        {
            audioSource.PlayOneShot(sonidoActivacion);
        }

        // Iniciar loop de sonido de atraccion
        if (sonidoAtraccion != null)
        {
            audioSource.clip = sonidoAtraccion;
            audioSource.loop = true;
            audioSource.Play();
        }

        // Iniciar deteccion de enemigos
        if (deteccionCoroutine != null)
            StopCoroutine(deteccionCoroutine);
        deteccionCoroutine = StartCoroutine(DeteccionContinua());

        // Iniciar contador de duracion
        if (duracionCoroutine != null)
            StopCoroutine(duracionCoroutine);
        duracionCoroutine = StartCoroutine(ContadorDuracion());

        if (showDebugLogs)
            Debug.Log($"EnemyMagnet: Iman activado por {duracionIman} segundos");
    }

    [ContextMenu("Desactivar Iman")]
    public void DesactivarIman()
    {
        if (!imanActivo)
            return;

        imanActivo = false;
        tiempoRestante = 0f;

        // Liberar todos los enemigos
        LiberarTodosLosEnemigos();

        // Desactivar efectos visuales
        if (efectoVisualIman != null)
            efectoVisualIman.SetActive(false);

        if (particulasAtraccion != null)
            particulasAtraccion.Stop();

        // Detener sonido de atraccion
        if (audioSource.isPlaying && audioSource.clip == sonidoAtraccion)
        {
            audioSource.Stop();
        }

        // Reproducir sonido de desactivacion
        if (sonidoDesactivacion != null)
        {
            audioSource.PlayOneShot(sonidoDesactivacion);
        }

        // Detener corrutinas
        if (deteccionCoroutine != null)
        {
            StopCoroutine(deteccionCoroutine);
            deteccionCoroutine = null;
        }

        if (duracionCoroutine != null)
        {
            StopCoroutine(duracionCoroutine);
            duracionCoroutine = null;
        }

        if (showDebugLogs)
            Debug.Log("EnemyMagnet: Iman desactivado");
    }

    private IEnumerator ContadorDuracion()
    {
        while (tiempoRestante > 0f)
        {
            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        // Tiempo agotado, desactivar iman
        DesactivarIman();
    }

    private IEnumerator DeteccionContinua()
    {
        while (imanActivo)
        {
            DetectarEnemigos();
            yield return new WaitForSeconds(intervaloDeteccion);
        }
    }

    private void DetectarEnemigos()
    {
        // Buscar todos los colliders en el rango
        Collider[] collidersEnRango = Physics.OverlapSphere(transform.position, rangoAtraccion, capasDeteccion);

        foreach (Collider col in collidersEnRango)
        {
            // Verificar si tiene el tag "Zombie" o el componente RagdollController
            bool esZombie = col.CompareTag("Zombie");
            RagdollController ragdoll = col.GetComponent<RagdollController>();

            if (esZombie || ragdoll != null)
            {
                GameObject enemigo = col.gameObject;

                // Si no está en la lista, agregarlo
                if (!enemigosAtraidos.Contains(enemigo))
                {
                    AtraerEnemigo(enemigo, ragdoll);
                }
            }
        }
    }

    private void AtraerEnemigo(GameObject enemigo, RagdollController ragdoll)
    {
        // Agregar a la lista
        enemigosAtraidos.Add(enemigo);

        // Activar ragdoll para efecto visual épico
        if (ragdoll != null && !ragdoll.EstaRagdollActivo())
        {
            ragdoll.ActivarMuerte();

            if (showDebugLogs)
                Debug.Log($"EnemyMagnet: Ragdoll activado en {enemigo.name}");
        }

        // Iniciar atracción del enemigo
        StartCoroutine(AtraerHaciaIman(enemigo));
    }

    private IEnumerator AtraerHaciaIman(GameObject enemigo)
    {
        if (enemigo == null) yield break;

        // Obtener todos los Rigidbodies del enemigo (para ragdoll)
        Rigidbody[] rigidbodies = enemigo.GetComponentsInChildren<Rigidbody>();
        Rigidbody mainRb = null;

        if (rigidbodies.Length == 0)
        {
            // Si no hay rigidbodies, buscar uno en el objeto principal
            mainRb = enemigo.GetComponent<Rigidbody>();
            if (mainRb != null)
            {
                rigidbodies = new Rigidbody[] { mainRb };
            }
            else
            {
                if (showDebugLogs)
                    Debug.LogWarning($"EnemyMagnet: {enemigo.name} no tiene Rigidbodies");
                yield break;
            }
        }
        else
        {
            // Encontrar el Rigidbody principal (el que tiene mas masa o el primero)
            mainRb = rigidbodies[0];
            foreach (Rigidbody rb in rigidbodies)
            {
                if (rb.mass > mainRb.mass)
                    mainRb = rb;
            }
        }

        // Atraer hacia el iman
        while (imanActivo && enemigo != null && enemigosAtraidos.Contains(enemigo))
        {
            // Usar la posicion del Rigidbody principal para calcular distancia
            Vector3 posicionEnemigo = mainRb != null ? mainRb.position : enemigo.transform.position;
            float distancia = Vector3.Distance(transform.position, posicionEnemigo);

            // Si llego suficientemente cerca, ejecutar muerte y liberar
            if (distancia <= distanciaLiberacion)
            {
                EjecutarMuerteZombie(enemigo);
                LiberarEnemigo(enemigo);
                yield break;
            }

            // Calcular direccion hacia el iman (SIEMPRE hacia la posicion actual)
            Vector3 direccion = (transform.position - posicionEnemigo).normalized;

            // Calcular velocidad necesaria para llegar
            float fuerzaAplicada = fuerzaAtraccion * (1f + (rangoAtraccion / Mathf.Max(distancia, 1f)));

            // Aplicar fuerza directa a todos los rigidbodies hacia el objetivo
            foreach (Rigidbody rb in rigidbodies)
            {
                if (rb != null)
                {
                    // Cancelar velocidad perpendicular al movimiento (para que no deriven)
                    Vector3 velocidadActual = rb.linearVelocity;
                    Vector3 velocidadHaciaObjetivo = Vector3.Project(velocidadActual, direccion);
                    Vector3 velocidadPerpendicular = velocidadActual - velocidadHaciaObjetivo;

                    // Reducir velocidad perpendicular (para evitar que giren sin control)
                    rb.linearVelocity = velocidadHaciaObjetivo + velocidadPerpendicular * 0.5f;

                    // Aplicar fuerza hacia el objetivo
                    rb.AddForce(direccion * fuerzaAplicada, ForceMode.Force);

                    // Opcional: Reducir velocidad angular para evitar que giren como locos
                    rb.angularVelocity *= 0.9f;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void EjecutarMuerteZombie(GameObject enemigo)
    {
        // Intentar obtener el componente Zombie
        Zombie zombieScript = enemigo.GetComponent<Zombie>();

        if (zombieScript != null)
        {
            // Ejecutar el metodo Morir del zombie
            zombieScript.Morir();

            if (showDebugLogs)
                Debug.Log($"EnemyMagnet: Muerte ejecutada en {enemigo.name}");
        }
        else
        {
            if (showDebugLogs)
                Debug.LogWarning($"EnemyMagnet: {enemigo.name} no tiene componente Zombie");
        }
    }

    private void LiberarEnemigo(GameObject enemigo)
    {
        if (enemigo != null && enemigosAtraidos.Contains(enemigo))
        {
            enemigosAtraidos.Remove(enemigo);

            if (showDebugLogs)
                Debug.Log($"EnemyMagnet: Enemigo liberado - {enemigo.name}");
        }
    }

    private void LiberarTodosLosEnemigos()
    {
        enemigosAtraidos.Clear();
    }

    // Métodos públicos
    public bool EstaActivo()
    {
        return imanActivo;
    }

    public float GetTiempoRestante()
    {
        return tiempoRestante;
    }

    public int GetCantidadEnemigosAtraidos()
    {
        return enemigosAtraidos.Count;
    }

    public void SetDuracion(float nuevaDuracion)
    {
        duracionIman = Mathf.Max(0.1f, nuevaDuracion);
    }

    public void SetRango(float nuevoRango)
    {
        rangoAtraccion = Mathf.Max(1f, nuevoRango);
    }

    public void SetFuerza(float nuevaFuerza)
    {
        fuerzaAtraccion = Mathf.Max(1f, nuevaFuerza);
    }

    // Gizmos para visualizar el rango
    private void OnDrawGizmosSelected()
    {
        // Dibujar esfera del rango de atraccion
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, rangoAtraccion);

        // Dibujar esfera mas opaca si esta activo
        if (imanActivo)
        {
            Color colorConAlpha = gizmoColor;
            colorConAlpha.a = 0.3f;
            Gizmos.color = colorConAlpha;
            Gizmos.DrawSphere(transform.position, rangoAtraccion);
        }

        // Dibujar distancia de liberacion
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaLiberacion);

        // Dibujar lineas hacia los enemigos atraidos
        if (Application.isPlaying && imanActivo)
        {
            Gizmos.color = Color.red;
            foreach (GameObject enemigo in enemigosAtraidos)
            {
                if (enemigo != null)
                {
                    Gizmos.DrawLine(transform.position, enemigo.transform.position);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Mostrar siempre un circulo pequeno en la posicion del iman
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    private void OnDestroy()
    {
        // Limpiar singleton
        if (singleton == this)
        {
            singleton = null;
        }

        // Asegurar que se liberen todos los enemigos
        if (imanActivo)
        {
            DesactivarIman();
        }
    }
}