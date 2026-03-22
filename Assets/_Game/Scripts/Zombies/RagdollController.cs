using UnityEngine;
using System.Collections.Generic;

public class RagdollController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Ragdoll Settings")]
    [SerializeField] private bool setupAutomatically = true; // Buscar automáticamente los componentes
    [SerializeField] private Transform ragdollRoot; // Raíz del ragdoll (opcional, si no se especifica usa este GameObject)

    [Header("Force Settings")]
    [SerializeField] private bool applyForceOnDeath = false;
    [SerializeField] private Vector3 deathForce = Vector3.zero;
    [SerializeField] private ForceMode forceMode = ForceMode.Impulse;
    [SerializeField] private string forceTargetBone = "Hips"; // Hueso al que aplicar la fuerza

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private List<Rigidbody> ragdollRigidbodies = new List<Rigidbody>();
    private List<Collider> ragdollColliders = new List<Collider>();
    private bool isRagdollActive = false;
    private Rigidbody mainRigidbody; // Rigidbody principal del personaje (si existe)
    private Collider mainCollider; // Collider principal del personaje (si existe)

    private void Awake()
    {
        // Obtener el animator si no está asignado
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("No se encontró un Animator en el GameObject!");
            }
        }

        // Guardar referencias al Rigidbody y Collider principales (si existen)
        mainRigidbody = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();

        if (setupAutomatically)
        {
            SetupRagdoll();
        }
    }

    private void Update()
    {
    }
    private void Start()
    {
        // Desactivar el ragdoll al inicio
        DesactivarRagdoll();
    }

    private void SetupRagdoll()
    {
        // Determinar la raíz desde donde buscar
        Transform root = ragdollRoot != null ? ragdollRoot : transform;

        // Buscar todos los Rigidbodies y Colliders en los hijos
        Rigidbody[] rigidbodies = root.GetComponentsInChildren<Rigidbody>();
        Collider[] colliders = root.GetComponentsInChildren<Collider>();

        ragdollRigidbodies.Clear();
        ragdollColliders.Clear();

        // Filtrar solo los que son parte del ragdoll (excluir el principal si existe)
        foreach (Rigidbody rb in rigidbodies)
        {
            // No incluir el Rigidbody principal del personaje
            if (rb != mainRigidbody)
            {
                ragdollRigidbodies.Add(rb);
            }
        }

        foreach (Collider col in colliders)
        {
            // No incluir el Collider principal del personaje
            if (col != mainCollider)
            {
                ragdollColliders.Add(col);
            }
        }

        if (showDebugLogs)
        {
            Debug.Log($"Ragdoll configurado: {ragdollRigidbodies.Count} Rigidbodies, {ragdollColliders.Count} Colliders");
        }
    }

    [ContextMenu("Activar Muerte")]
    public void ActivarMuerte()
    {
        ActivarMuerte(Vector3.zero);
    }

    // Método sobrecargado que permite pasar una fuerza personalizada
    public void ActivarMuerte(Vector3 fuerza)
    {
        if (isRagdollActive)
        {
            if (showDebugLogs)
                Debug.LogWarning("El ragdoll ya está activo!");
            return;
        }

        isRagdollActive = true;

        // Desactivar el Animator
        if (animator != null)
        {
            animator.enabled = false;
            if (showDebugLogs)
                Debug.Log("Animator desactivado");
        }

        // Desactivar el Rigidbody principal si existe
        if (mainRigidbody != null)
        {
            mainRigidbody.isKinematic = true;
        }

        // Desactivar el Collider principal si existe
        if (mainCollider != null)
        {
            mainCollider.enabled = false;
        }

        // Activar todos los componentes del ragdoll
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }

        // Aplicar fuerza si está configurado
        if (fuerza != Vector3.zero)
        {
            AplicarFuerza(fuerza);
        }
        else if (applyForceOnDeath && deathForce != Vector3.zero)
        {
            AplicarFuerza(deathForce);
        }

        if (showDebugLogs)
        {
            Debug.Log("Ragdoll activado!");
        }
    }

    private void DesactivarRagdoll()
    {
        // Desactivar todos los Rigidbodies del ragdoll
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }

        // Desactivar todos los Colliders del ragdoll
        foreach (Collider col in ragdollColliders)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }

        // Asegurar que el Animator esté activo
        if (animator != null)
        {
            animator.enabled = true;
        }

        // Asegurar que los componentes principales estén activos
        if (mainRigidbody != null)
        {
            mainRigidbody.isKinematic = false;
        }

        if (mainCollider != null)
        {
            mainCollider.enabled = true;
        }

        isRagdollActive = false;

        if (showDebugLogs)
        {
            Debug.Log("Ragdoll desactivado");
        }
    }

    private void AplicarFuerza(Vector3 fuerza)
    {
        // Buscar el hueso específico o aplicar a todos
        if (!string.IsNullOrEmpty(forceTargetBone))
        {
            // Buscar el hueso específico
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                if (rb != null && rb.name.Contains(forceTargetBone))
                {
                    rb.AddForce(fuerza, forceMode);
                    if (showDebugLogs)
                        Debug.Log($"Fuerza aplicada a {rb.name}: {fuerza}");
                    return;
                }
            }
        }

        // Si no se encontró el hueso específico, aplicar al primer Rigidbody
        if (ragdollRigidbodies.Count > 0 && ragdollRigidbodies[0] != null)
        {
            ragdollRigidbodies[0].AddForce(fuerza, forceMode);
            if (showDebugLogs)
                Debug.Log($"Fuerza aplicada a {ragdollRigidbodies[0].name}: {fuerza}");
        }
    }

    // Método público para aplicar fuerza desde un punto específico (útil para atropellos)
    public void ActivarMuerteConImpacto(Vector3 puntoImpacto, Vector3 direccion, float fuerzaImpacto)
    {
        if (isRagdollActive) return;

        // Activar el ragdoll primero
        ActivarMuerte();

        // Encontrar el Rigidbody más cercano al punto de impacto
        Rigidbody rbMasCercano = null;
        float distanciaMinima = float.MaxValue;

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb != null)
            {
                float distancia = Vector3.Distance(rb.position, puntoImpacto);
                if (distancia < distanciaMinima)
                {
                    distanciaMinima = distancia;
                    rbMasCercano = rb;
                }
            }
        }

        // Aplicar la fuerza al Rigidbody más cercano
        if (rbMasCercano != null)
        {
            rbMasCercano.AddForce(direccion.normalized * fuerzaImpacto, ForceMode.Impulse);
            if (showDebugLogs)
                Debug.Log($"Impacto aplicado a {rbMasCercano.name} con fuerza {fuerzaImpacto}");
        }
    }

    // Método para resetear el ragdoll (útil para testing)
    [ContextMenu("Resetear Ragdoll")]
    public void ResetearRagdoll()
    {
        DesactivarRagdoll();
        if (showDebugLogs)
            Debug.Log("Ragdoll reseteado");
    }

    // Método para reconfigurar el ragdoll
    [ContextMenu("Reconfigurar Ragdoll")]
    public void ReconfigurarRagdoll()
    {
        SetupRagdoll();
        DesactivarRagdoll();
    }

    // Getters
    public bool EstaRagdollActivo()
    {
        return isRagdollActive;
    }

    public int GetCantidadRigidbodies()
    {
        return ragdollRigidbodies.Count;
    }

    public int GetCantidadColliders()
    {
        return ragdollColliders.Count;
    }

    // Método para obtener todos los Rigidbodies (útil para efectos adicionales)
    public List<Rigidbody> GetRagdollRigidbodies()
    {
        return new List<Rigidbody>(ragdollRigidbodies);
    }
}