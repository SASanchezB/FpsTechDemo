using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public interface IPoolable
{
    void OnPooledObjectReused(); // Se llama cuando se activa desde el pool
}

public class Grenade : MonoBehaviour, IPoolable
{
    [Header("Explosion Settings")]
    public float highDamageRadius = 2f;
    public float midDamageRadius = 4f;
    public float lowDamageRadius = 6f;

    public int highDamage = 100;
    public int midDamage = 60;
    public int lowDamage = 30;

    public float explosionDelay = 1f;

    [Header("Explosion Effect")]
    public GameObject explosionEffectPrefab;

    private Rigidbody rb;
    private bool hasExploded = false;
    private bool collisionHandled = false;

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
        // Reiniciar estados internos al salir del pool
        hasExploded = false;
        collisionHandled = false;

        if (rb == null) rb = GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.linearDamping = 0;
        rb.angularDamping = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionHandled)
            return;

        collisionHandled = true;

        rb.linearVelocity *= 0.5f;
        rb.angularVelocity *= 0.2f;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                rb.linearDamping = 4f;
                rb.angularDamping = 4f;
                break;
            }
        }

        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    private void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;

        // Efecto visual
        if (explosionEffectPrefab != null)
        {
            GameObject fx = ObjectPool.Instance.GetPooledObject(explosionEffectPrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = fx.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play();
        }

        HashSet<object> alreadyDamaged = new HashSet<object>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, lowDamageRadius);
        foreach (Collider nearbyObject in colliders)
        {
            float distance = Vector3.Distance(transform.position, nearbyObject.transform.position);

            var player = nearbyObject.GetComponentInParent<HealthControllerPlayer>();
            var enemy = nearbyObject.GetComponentInParent<HealthControllerEnemy>();

            if (enemy != null && !alreadyDamaged.Contains(enemy))
            {
                alreadyDamaged.Add(enemy);

                if (distance <= highDamageRadius)
                    enemy.TakeDamage(highDamage);
                else if (distance <= midDamageRadius)
                    enemy.TakeDamage(midDamage);
                else if (distance <= lowDamageRadius)
                    enemy.TakeDamage(lowDamage);
            }

            if (player != null && !alreadyDamaged.Contains(player))
            {
                alreadyDamaged.Add(player);

                if (distance <= highDamageRadius)
                    player.TakeDamage(highDamage);
                else if (distance <= midDamageRadius)
                    player.TakeDamage(midDamage);
                else if (distance <= lowDamageRadius)
                    player.TakeDamage(lowDamage);
            }
        }

        // En vez de destruir, lo apagamos para reusar desde el pool
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // High
        Gizmos.DrawSphere(transform.position, highDamageRadius);

        Gizmos.color = new Color(1f, 1f, 0f, 0.3f); // Mid
        Gizmos.DrawSphere(transform.position, midDamageRadius);

        Gizmos.color = new Color(0f, 0f, 1f, 0.3f); // Low
        Gizmos.DrawSphere(transform.position, lowDamageRadius);
    }
}
