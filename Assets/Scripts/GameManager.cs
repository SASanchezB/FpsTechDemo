using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private float timeBetweenSpawns = 2f;
    [SerializeField] private int maxEnemiesInScene = 3;
    [SerializeField] private ParticleSystem spawnParticle;

    [System.Serializable]
    public class EnemyStats
    {
        public int health;
    }

    [SerializeField] private List<EnemyStats> enemyStatsList;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    private Transform playerTransform;
    public List<GameObject> activeEnemies = new List<GameObject>();
    private Coroutine spawnCoroutine;

    private EnemyFactory[] enemyFactories;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeReferences();
    }

    private void Start()
    {
        InitializeReferences();

        if (spawnCoroutine == null)
            spawnCoroutine = StartCoroutine(SpawnEnemyWithDelay());
    }

    private void InitializeReferences()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player no encontrado en escena.");
        }

        enemyFactories = FindObjectsByType<EnemyFactory>(FindObjectsSortMode.None);

        spawnPoints.Clear();
        GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        foreach (GameObject obj in spawnObjects)
        {
            spawnPoints.Add(obj.transform);
        }

        Debug.Log($"[GameManager] Referencias actualizadas: {enemyFactories.Length} factories, {spawnPoints.Count} puntos de spawn.");
    }

    private IEnumerator SpawnEnemyWithDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            if (activeEnemies.Count < maxEnemiesInScene)
                SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (playerTransform == null || enemyFactories.Length == 0 || spawnPoints.Count == 0) return;

        Vector3 spawnPosition = GetClosestSpawnPosition();

        int enemyType = Random.Range(0, enemyFactories.Length);
        if (enemyType >= enemyStatsList.Count) return;

        EnemyStats stats = enemyStatsList[enemyType];
        GameObject enemy = enemyFactories[enemyType].CreateEnemy(spawnPosition, stats.health);

        if (enemy == null) return;

        activeEnemies.Add(enemy);

        HealthControllerEnemy health = enemy.GetComponent<HealthControllerEnemy>();
        if (health != null)
        {
            health.onDeath += () => RemoveEnemyFromList(enemy);
        }

        if (spawnParticle != null)
        {
            ParticleSystem ps = Instantiate(spawnParticle, spawnPosition, Quaternion.identity);
            ps.Play();
        }
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            StartCoroutine(RespawnEnemyAfterDelay());
        }
    }

    private IEnumerator RespawnEnemyAfterDelay()
    {
        yield return new WaitForSeconds(timeBetweenSpawns);
        if (activeEnemies.Count < maxEnemiesInScene)
            SpawnEnemy();
    }

    private Vector3 GetClosestSpawnPosition()
    {
        if (spawnPoints.Count == 0) return Vector3.zero;

        Vector3 closest = spawnPoints[0].position;
        float minDistance = Vector3.Distance(playerTransform.position, closest);

        foreach (Transform spawn in spawnPoints)
        {
            float dist = Vector3.Distance(playerTransform.position, spawn.position);
            if (dist < minDistance)
            {
                closest = spawn.position;
                minDistance = dist;
            }
        }

        return closest;
    }
}
