using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    public GameObject CreateEnemy(Vector3 spawnPosition, int health)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyFactory no tiene un prefab asignado.");
            return null;
        }

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        HealthControllerEnemy healthController = enemy.GetComponent<HealthControllerEnemy>();
        if (healthController != null)
        {
            healthController.SetHealth(health);
        }

        EnemyAi enemyAi = enemy.GetComponent<EnemyAi>();
        if (enemyAi != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                enemyAi.SetTarget(player.transform);
            }
        }

        return enemy;
    }
}
