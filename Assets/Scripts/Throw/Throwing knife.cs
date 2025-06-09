using UnityEngine;

public class ThrowingKnife : MonoBehaviour, IPoolable
{
    public int damage = 75;

    [Header("Impact Effects")]
    public GameObject enemyHitEffectPrefab;    // Partícula para impacto en enemigos
    public GameObject defaultHitEffectPrefab;  // Partícula para impacto en otras superficies

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

        transform.SetParent(null);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit)
            return;

        hasHit = true;

        GameObject hitObject = collision.gameObject;
        ContactPoint contact = collision.contacts[0];

        // Si golpea a un enemigo
        if (hitObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            HealthControllerEnemy enemyHealth = hitObject.GetComponent<HealthControllerEnemy>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log("Cuchillo DAÑO");

                // Partícula de impacto en enemigo
                if (enemyHitEffectPrefab != null)
                    Instantiate(enemyHitEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));

                gameObject.SetActive(false);
            }
        }
        else
        {
            // Partícula de impacto genérica
            if (defaultHitEffectPrefab != null)
                Instantiate(defaultHitEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
        }

        // Clava el cuchillo
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.SetParent(null);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasHit) return;

        Debug.Log($"Trigger con {other.name}");

        ThrowingMechanic mechanic = other.GetComponentInParent<ThrowingMechanic>();
        if (mechanic != null)
        {
            if (mechanic.throwUpwardForce < 1)
            {
                mechanic.totalThrows++;
                mechanic.UpdateUI();
            }

            gameObject.SetActive(false); // Reutilizable
        }
    }
}
