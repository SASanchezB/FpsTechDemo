using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    public int poolSize = 20;
    public float lifetime = 10f;

    private Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void EnsurePoolExists(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab))
        {
            Queue<GameObject> newPool = new Queue<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                newPool.Enqueue(obj);
            }
            pools[prefab] = newPool;
        }
    }

    public GameObject GetPooledObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        EnsurePoolExists(prefab);
        Queue<GameObject> pool = pools[prefab];
        GameObject instance = null;

        int checkedCount = 0;
        while (checkedCount < pool.Count)
        {
            GameObject candidate = pool.Dequeue();
            if (!candidate.activeInHierarchy)
            {
                instance = candidate;
                break;
            }
            else
            {
                pool.Enqueue(candidate);
                checkedCount++;
            }
        }

        if (instance == null)
        {
            instance = pool.Dequeue();
            instance.SetActive(false);
        }

        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.transform.localScale = prefab.transform.localScale;
        instance.SetActive(true);

        pool.Enqueue(instance);

        // Solo desactivar automáticamente si no tiene lógica personalizada
        if (instance.GetComponent<IPoolable>() == null)
        {
            StartCoroutine(DeactivateAfterTime(instance));
        }

        return instance;
    }

    private IEnumerator DeactivateAfterTime(GameObject obj)
    {
        yield return new WaitForSeconds(lifetime);
        if (obj != null)
            obj.SetActive(false);
    }
}
