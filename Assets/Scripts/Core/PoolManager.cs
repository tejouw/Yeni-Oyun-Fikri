using UnityEngine;
using System.Collections.Generic;

namespace NeonSurvivors.Core
{
    public class PoolManager : MonoBehaviour
    {
        private static PoolManager _instance;
        public static PoolManager Instance => _instance;

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        [Header("Pool Configuration")]
        public List<Pool> pools = new List<Pool>();

        private Dictionary<string, Queue<GameObject>> poolDictionary;
        private Dictionary<string, GameObject> prefabDictionary;
        private Dictionary<string, Transform> poolParents;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializePools();
        }

        void InitializePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            prefabDictionary = new Dictionary<string, GameObject>();
            poolParents = new Dictionary<string, Transform>();

            foreach (Pool pool in pools)
            {
                if (pool.prefab == null)
                {
                    Debug.LogWarning($"Pool {pool.tag} has null prefab, skipping");
                    continue;
                }

                // Create parent object for organization
                GameObject poolParent = new GameObject($"Pool_{pool.tag}");
                poolParent.transform.SetParent(transform);
                poolParents[pool.tag] = poolParent.transform;

                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, poolParent.transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
                prefabDictionary.Add(pool.tag, pool.prefab);

                Debug.Log($"Initialized pool '{pool.tag}' with {pool.size} objects");
            }
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag '{tag}' doesn't exist");
                return null;
            }

            GameObject objectToSpawn = poolDictionary[tag].Dequeue();

            if (objectToSpawn == null)
            {
                Debug.LogWarning($"Pool '{tag}' returned null object, creating new one");
                objectToSpawn = Instantiate(prefabDictionary[tag], poolParents[tag]);
            }

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            poolDictionary[tag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }

        public void ReturnToPool(string tag, GameObject obj)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            obj.transform.SetParent(poolParents[tag]);
        }

        public void ClearPool(string tag)
        {
            if (!poolDictionary.ContainsKey(tag))
                return;

            Queue<GameObject> pool = poolDictionary[tag];
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                if (obj != null)
                    Destroy(obj);
            }

            poolDictionary.Remove(tag);
            prefabDictionary.Remove(tag);
        }

        public void ClearAllPools()
        {
            foreach (var kvp in poolDictionary)
            {
                while (kvp.Value.Count > 0)
                {
                    GameObject obj = kvp.Value.Dequeue();
                    if (obj != null)
                        Destroy(obj);
                }
            }

            poolDictionary.Clear();
            prefabDictionary.Clear();
        }
    }
}
