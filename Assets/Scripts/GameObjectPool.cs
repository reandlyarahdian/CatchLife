using UnityEngine;
using UnityEngine.Pool;

// Note that this class is not a MonoBehavior. Just a pure C# class.
public class GameObjectPool
{
    GameObject prefab;
    ObjectPool<GameObject> pool;
    int defaultSize;
    int maxSize;

    // Our class's constructor. Takes the prefab to spawn as an argument.
    public GameObjectPool(GameObject prefab, int defaultSize = 1, int maxSize = 1)
    {
        this.prefab = prefab;  // The prefab to spawn.
        this.defaultSize = defaultSize;  // Pool's starting number of objects.
        this.maxSize = maxSize;  // Max size for our pool.
        // Initializing our pool.
        pool = new ObjectPool<GameObject>(
            CreatePooledObject,
            OnGetFromPool,
            OnReturnToPool,
            OnDestroyPooledObject,
            true,
            defaultSize,
            maxSize
            );
    }

    // Wrapper function for pool.Get. Gets object and sets position.
    // Also resets rigidbody's velocity if it has one.
    public GameObject GetObject(Vector3 position)
    {
        GameObject obj = pool.Get();
        obj.transform.position = position;
        if (obj.TryGetComponent<CharacterFall>(out CharacterFall cf))
        {
            cf.Init();
        }
        return obj;
    }

    // Wrapper function for pool.Release
    public void ReleaseObject(GameObject obj)
    {
        pool.Release(obj);
    }

    // Return a brand new GameObject instance for our pool to use.
    // We have to specify GameObject.Instantiate because this isn't a Monobehavior.
    GameObject CreatePooledObject()
    {
        GameObject newObject = GameObject.Instantiate(prefab);
        return newObject;
    }

    // When an object is taken from the pool, activate it.
    void OnGetFromPool(GameObject pooledObject)
    {
        pooledObject.SetActive(true);
    }

    // When an object is returned to the pool, deactivate it.
    void OnReturnToPool(GameObject pooledObject)
    {
        pooledObject.SetActive(false);
    }

    // When the pool discards an object, destroy the GameObject.
    public void OnDestroyPooledObject(GameObject pooledObject)
    {
        GameObject.Destroy(pooledObject);
    }
}
