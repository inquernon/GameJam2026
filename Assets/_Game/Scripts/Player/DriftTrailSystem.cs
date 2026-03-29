using UnityEngine;
using System.Collections.Generic;

public class DriftTrailSystem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private MotoController motoController;
    [SerializeField] private Transform puntoContactoSuelo; // Punto donde toca la rueda trasera

    [Header("Trail Settings")]
    [SerializeField] private Material trailMaterial;
    [SerializeField] private float trailWidth = 0.15f;
    [SerializeField] private Color trailColor = Color.black;
    [SerializeField] private float minDistanciaEntreIntos = 0.1f;
    [SerializeField] private float alturaDelSuelo = 0.02f; // Altura sobre el suelo

    [Header("Drift Detection")]
    [SerializeField] private float anguloMinimoDrift = 15f; // Ángulo mínimo para considerar derrape
    [SerializeField] private float velocidadMinimaDrift = 5f; // Velocidad mínima para dejar marca

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoDerrape;
    [SerializeField] private float volumenDerrape = 0.7f;
    [SerializeField] private float fadeSpeed = 5f; // Velocidad de fade in/out del sonido

    [Header("Performance")]
    [SerializeField] private int maxPuntosLineRenderer = 500;
    [SerializeField] private float tiempoVidaMarca = 30f; // Tiempo que permanece la marca

    private List<LineRenderer> lineasActivas = new List<LineRenderer>();
    private LineRenderer lineaActual;
    private Vector3 ultimaPosicion;
    private bool estaDerrapando = false;
    private float targetVolume = 0f;

    private void Awake()
    {
        // Auto-detectar componentes si no están asignados
        if (motoController == null)
            motoController = GetComponent<MotoController>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0f;
        }

        if (sonidoDerrape != null)
            audioSource.clip = sonidoDerrape;
    }

    private void Update()
    {
        bool debeDejar = DebeDejarMarca();

        if (debeDejar && !estaDerrapando)
        {
            IniciarDerrape();
        }
        else if (!debeDejar && estaDerrapando)
        {
            DetenerDerrape();
        }
        else if (debeDejar && estaDerrapando)
        {
            ActualizarDerrape();
        }

        // Actualizar volumen del sonido suavemente
        ActualizarSonido();
    }

    private bool DebeDejarMarca()
    {
        if (motoController == null) return false;

        // Verificar velocidad mínima
        if (motoController.CurrentSpeed < velocidadMinimaDrift)
            return false;

        // Calcular ángulo de derrape (diferencia entre dirección de movimiento y rotación)
        Vector3 direccionMovimiento = transform.forward;
        Vector3 velocidadDireccion = motoController.GetComponent<Rigidbody>().linearVelocity.normalized;

        if (velocidadDireccion.magnitude < 0.1f)
            return false;

        float angulo = Vector3.Angle(direccionMovimiento, velocidadDireccion);

        // Está derrapando si el ángulo es mayor al mínimo
        return angulo >= anguloMinimoDrift;
    }

    private void IniciarDerrape()
    {
        estaDerrapando = true;

        // Crear nuevo LineRenderer
        GameObject lineObj = new GameObject("DriftTrail");
        lineObj.transform.SetParent(null); // No hijo para que no se mueva con la moto

        lineaActual = lineObj.AddComponent<LineRenderer>();
        ConfigurarLineRenderer(lineaActual);

        lineasActivas.Add(lineaActual);

        // Iniciar con el primer punto
        Vector3 posicion = ObtenerPosicionSuelo();
        lineaActual.positionCount = 1;
        lineaActual.SetPosition(0, posicion);
        ultimaPosicion = posicion;

        // Iniciar sonido
        if (sonidoDerrape != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        targetVolume = volumenDerrape;

        // Destruir después de un tiempo
        Destroy(lineObj, tiempoVidaMarca);
    }

    private void ActualizarDerrape()
    {
        if (lineaActual == null) return;

        Vector3 posicionActual = ObtenerPosicionSuelo();

        // Solo añadir punto si se ha movido una distancia mínima
        if (Vector3.Distance(posicionActual, ultimaPosicion) >= minDistanciaEntreIntos)
        {
            // Verificar límite de puntos
            if (lineaActual.positionCount >= maxPuntosLineRenderer)
            {
                // Iniciar nueva línea
                DetenerDerrape();
                IniciarDerrape();
                return;
            }

            int nuevoIndice = lineaActual.positionCount;
            lineaActual.positionCount = nuevoIndice + 1;
            lineaActual.SetPosition(nuevoIndice, posicionActual);
            ultimaPosicion = posicionActual;
        }
    }

    private void DetenerDerrape()
    {
        estaDerrapando = false;
        lineaActual = null;

        // Fade out del sonido
        targetVolume = 0f;
    }

    private void ActualizarSonido()
    {
        if (audioSource == null || sonidoDerrape == null) return;

        // Suavizar el volumen
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);

        // Detener el audio si el volumen es muy bajo
        if (audioSource.volume < 0.01f && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private Vector3 ObtenerPosicionSuelo()
    {
        Vector3 posicion;

        if (puntoContactoSuelo != null)
        {
            posicion = puntoContactoSuelo.position;
        }
        else
        {
            // Usar la posición de la moto
            posicion = transform.position;
        }

        // Raycast para detectar el suelo
        RaycastHit hit;
        if (Physics.Raycast(posicion + Vector3.up, Vector3.down, out hit, 5f))
        {
            posicion = hit.point + Vector3.up * alturaDelSuelo;
        }
        else
        {
            posicion.y = alturaDelSuelo;
        }

        return posicion;
    }

    private void ConfigurarLineRenderer(LineRenderer lr)
    {
        lr.material = trailMaterial != null ? trailMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = trailWidth;
        lr.endWidth = trailWidth;
        lr.startColor = trailColor;
        lr.endColor = trailColor;
        lr.useWorldSpace = true;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.alignment = LineAlignment.TransformZ;

        // Configuración para que se vea bien en el suelo
        lr.textureMode = LineTextureMode.Tile;
        lr.numCornerVertices = 5;
        lr.numCapVertices = 5;
    }

    // Método para limpiar todas las marcas
    [ContextMenu("Limpiar Todas las Marcas")]
    public void LimpiarTodasLasMarcas()
    {
        foreach (LineRenderer lr in lineasActivas)
        {
            if (lr != null)
                Destroy(lr.gameObject);
        }

        lineasActivas.Clear();
        lineaActual = null;
        estaDerrapando = false;
    }

    // Método para forzar inicio de derrape (para testing)
    [ContextMenu("Test Derrape")]
    public void TestDerrape()
    {
        if (!estaDerrapando)
            IniciarDerrape();
    }

    // Getters públicos
    public bool EstaDerrapando()
    {
        return estaDerrapando;
    }

    public int GetCantidadMarcas()
    {
        return lineasActivas.Count;
    }

    private void OnDestroy()
    {
        // Limpiar al destruir
        LimpiarTodasLasMarcas();
    }

    private void OnDrawGizmos()
    {
        // Visualizar el punto de contacto con el suelo
        if (puntoContactoSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoContactoSuelo.position, 0.1f);
        }
    }
}