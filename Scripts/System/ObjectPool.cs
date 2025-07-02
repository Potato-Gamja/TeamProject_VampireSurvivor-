using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public List<Pool> enemyPools;
    public List<Pool> expgemPools;
    public List<Pool> damageTextPools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, Queue<GameObject>> poolDictionary_Enemy;
    private Dictionary<string, Queue<GameObject>> poolDictionary_Expgem;
    private Dictionary<string, Queue<GameObject>> poolDictionary_DamageText;
    private Dictionary<GameObject, DamageText> damageTextComponentMap = new();


    private void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolDictionary_Enemy = new Dictionary<string, Queue<GameObject>>();
        poolDictionary_Expgem = new Dictionary<string, Queue<GameObject>>();
        poolDictionary_DamageText = new Dictionary<string, Queue<GameObject>>();

        //무기 풀링
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = CreateNewObject(pool.prefab);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }

        //몬스터 풀링
        foreach (Pool pool in enemyPools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = CreateNewObject_Enemy(pool.prefab);
                objectPool.Enqueue(obj);
            }

            poolDictionary_Enemy.Add(pool.tag, objectPool);
        }

        //경험치 잼 풀링
        foreach (Pool pool in expgemPools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = CreateNewObject_Enemy(pool.prefab);
                objectPool.Enqueue(obj);
            }

            poolDictionary_Expgem.Add(pool.tag, objectPool);
        }

        //데미지 텍스트 잼 풀링
        foreach (Pool pool in damageTextPools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = CreateNewObject_DamageText(pool.prefab);
                objectPool.Enqueue(obj);
            }

            poolDictionary_DamageText.Add(pool.tag, objectPool);
        }
    }

    //무기 풀링
    private GameObject CreateNewObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        
        // 보팔검 이펙트인 경우 플레이어를 부모로 설정
        if(prefab.name.Contains("SwordEffect"))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                obj.transform.SetParent(player.transform, false);
                Debug.Log("VorpalSwordEffect parent set to Player");
            }
            else
            {
                Debug.LogWarning("Player not found for VorpalSwordEffect parent setting");
            }
        }
        
        obj.SetActive(false);
        return obj;
    }
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];
        
        // 비활성화된 오브젝트가 있는지 확인
        GameObject objectToSpawn = null;
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                objectToSpawn = obj;
                break;
            }
        }

        // 비활성화된 오브젝트가 없으면 새로 생성
        if (objectToSpawn == null)
        {
            Pool poolSettings = pools.Find(p => p.tag == tag);
            objectToSpawn = CreateNewObject(poolSettings.prefab);
            pool.Enqueue(objectToSpawn);
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        
        return objectToSpawn;
    }
    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return;
        }

        objectToReturn.SetActive(false);
    }


    //몬스터 풀링
    private GameObject CreateNewObject_Enemy(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);

        obj.SetActive(false);
        return obj;
    }
    public GameObject SpawnFromPool_Enemy(string tag, Vector3 position)
    {
        if (!poolDictionary_Enemy.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary_Enemy[tag];

        // 비활성화된 오브젝트가 있는지 확인
        GameObject objectToSpawn = null;
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                objectToSpawn = obj;
                break;
            }
        }

        // 비활성화된 오브젝트가 없으면 새로 생성
        if (objectToSpawn == null)
        {
            Pool poolSettings = enemyPools.Find(p => p.tag == tag);
            objectToSpawn = CreateNewObject(poolSettings.prefab);
            pool.Enqueue(objectToSpawn);
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        return objectToSpawn;
    }
    public void ReturnToPool_Enemy(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary_Enemy.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return;
        }

        objectToReturn.SetActive(false);
    }


    //경험치 잼 풀링
    private GameObject CreateNewObject_Expgem(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);

        obj.SetActive(false);
        return obj;
    }
    public GameObject SpawnFromPool_Expgem(string tag, Vector3 position)
    {
        if (!poolDictionary_Expgem.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary_Expgem[tag];

        // 비활성화된 오브젝트가 있는지 확인
        GameObject objectToSpawn = null;
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                objectToSpawn = obj;
                break;
            }
        }

        // 비활성화된 오브젝트가 없으면 새로 생성
        if (objectToSpawn == null)
        {
            Pool poolSettings = expgemPools.Find(p => p.tag == tag);
            objectToSpawn = CreateNewObject(poolSettings.prefab);
            pool.Enqueue(objectToSpawn);
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        return objectToSpawn;
    }
    public void ReturnToPool_Expgem(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary_Expgem.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return;
        }

        objectToReturn.SetActive(false);
    }


    //데미지 텍스트 풀링
    private GameObject CreateNewObject_DamageText(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);

        DamageText dt = obj.GetComponent<DamageText>();
        if (dt != null)
        {
            damageTextComponentMap[obj] = dt;
        }

        return obj;
    }
    public GameObject SpawnFromPool_DamageText(string tag, Vector3 position, float damage)
    {
        if (!poolDictionary_DamageText.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary_DamageText[tag];
        GameObject objectToSpawn = null;

        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                objectToSpawn = obj;
                break;
            }
        }

        if (objectToSpawn == null)
        {
            Pool poolSettings = damageTextPools.Find(p => p.tag == tag);
            objectToSpawn = CreateNewObject_DamageText(poolSettings.prefab);
            pool.Enqueue(objectToSpawn);
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;

        if (!damageTextComponentMap.TryGetValue(objectToSpawn, out DamageText dt))
        {
            dt = objectToSpawn.GetComponent<DamageText>();
            damageTextComponentMap[objectToSpawn] = dt;
        }

        dt?.Setup(damage);

        return objectToSpawn;
    }
    public void ReturnToPool_DamageText(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary_DamageText.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return;
        }

        objectToReturn.SetActive(false);
    }
} 