using UnityEngine;

public class HealthControllerEnemy : MonoBehaviour
{
    public int maxHealth = 100;
    private HealthManager healthManager;

    public System.Action onDeath; // Evento que se disparará al morir

    [Header("Effects")]
    public GameObject deathParticlePrefab; // Prefab de la partícula de muerte

    private void Start()
    {
        InitializeHealth();
    }

    private void InitializeHealth()
    {
        healthManager = new HealthManager(maxHealth);
        healthManager.onDeath += Die; // Se suscribe al evento
    }

    public void TakeDamage(int amount)
    {
        healthManager.TakeDamage(amount);
        Debug.Log(healthManager.currentHealth);
    }

    public void Heal(int amount)
    {
        healthManager.Heal(amount);
    }

    private void Die()
    {
        onDeath?.Invoke(); // Notifica al GameManager u otros sistemas

        // Instanciar la partícula si está asignada
        if (deathParticlePrefab != null)
        {
            GameObject particle = Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);

            // Si tiene sistema de partículas, lo activamos y destruimos después
            ParticleSystem ps = particle.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                Destroy(particle, ps.main.duration + ps.main.startLifetime.constantMax); // Destruye tras reproducirse
            }
            else
            {
                Destroy(particle, 2f); // Valor de fallback
            }
        }

        // Aquí asumimos que solo hay un jugador por ahora
        PointsManager.Instance?.AddPoints(100);

        Destroy(gameObject);
    }

    public void SetHealth(int value)
    {
        maxHealth = value;
        InitializeHealth(); // Se asegura de inicializar correctamente el sistema de salud
    }
}
