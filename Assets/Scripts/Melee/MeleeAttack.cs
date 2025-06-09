using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] int damage = 25;
    private bool hasHit = false;

    private void OnEnable()
    {
        hasHit = false; // Reseteamos el estado en cada nuevo ataque
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return; // Ya golpeó a alguien, no sigue

        // Solo seguimos si la capa es "Enemy"
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            return;

        // Intentar obtener el componente de salud en el enemigo o su padre
        HealthControllerEnemy enemy = other.GetComponentInParent<HealthControllerEnemy>();
        if (enemy != null)
        {
            Debug.Log("✅ Dañando a: " + other.name);
            enemy.TakeDamage(damage);
            hasHit = true; // Solo dañamos una vez
        }
    }
}
