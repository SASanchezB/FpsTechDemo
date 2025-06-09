using UnityEngine;

public class ThrowingKnife : MonoBehaviour, IPoolable
{
    public int damage = 75;

    private Rigidbody rb;
    private bool hasHit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        OnPooledObjectReused();
    }

    public void OnPooledObjectReused()
    {
        hasHit = false;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.linearDamping = 0;
        rb.angularDamping = 0;

        transform.SetParent(null); // Se asegura de estar libre
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit)
            return;

        hasHit = true;

        GameObject hitObject = collision.gameObject;

        if (hitObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            HealthControllerEnemy enemyHealth = hitObject.GetComponent<HealthControllerEnemy>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                gameObject.SetActive(false);
                Debug.Log("Cuchillo DA�O");
            }
        }

        // El cuchillo se pega en el punto de impacto
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetParent(null); // Opcional: puedes hacer que se "pegue" al enemigo
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasHit) return;

        Debug.Log($"Trigger con {other.name}");

        ThrowingMechanic mechanic = other.GetComponentInParent<ThrowingMechanic>();
        if (mechanic != null)
        {
            if (mechanic.throwUpwardForce < 1) // Si es cuchillo (usando tu l�gica)
            {
                mechanic.totalThrows++;
                mechanic.UpdateUI();
            }

            gameObject.SetActive(false); // NO destruir, se reutiliza
        }
    }
}
