using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 5f; // Tiempo antes de destruirse automáticamente
    public LayerMask whatIsPlayer; // Asignar en el Inspector

    private HealthControllerPlayer playerHealth;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player"); // Asegúrate de que el jugador tiene el tag "Player"
        if (player != null)
        {
            playerHealth = player.GetComponent<HealthControllerPlayer>();
        }
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Impacto con: {other.gameObject.name}, Layer: {LayerMask.LayerToName(other.gameObject.layer)}");

        if (((1 << other.gameObject.layer) & whatIsPlayer) != 0)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
                //Debug.Log("Daño aplicado al jugador.");
            }
            else
            {
                //Debug.LogError("No se encontró HealthControllerPlayer en el jugador.");
            }

            Destroy(gameObject);
        }
    }

}
