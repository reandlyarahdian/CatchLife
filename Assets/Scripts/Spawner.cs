using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private StatSO[] prefab;  // The object to spawn.
    Bounds bounds;  // Our spawn area's boundaries.

    private GameObjectPool objPool;  // Our new pool!

    private Coroutine coroutine;

    static Spawner instance;

    public static Spawner Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("Spawner").GetComponent<Spawner>();

            return instance;
        }
    }

    // Start is called before the first frame update
    public void Init()
    {
        bounds = GetComponent<Collider>().bounds;
        // Initializing our new pool with our marble prefab, default size 25, and max size 100.
        if (coroutine == null)
        {
            coroutine = StartCoroutine(Spawn());
        }
        else
        {
            StopCoroutine(coroutine);

            coroutine = StartCoroutine(Spawn());
        }
    }

    // Spawn marbles every x seconds, x=cooldown.
    private IEnumerator Spawn()
    {
        while (true)
        {
            objPool = new GameObjectPool(prefab[Random.Range(0, prefab.Length)].CharacterToSpawn(), 1, 1);
            yield return new WaitForSeconds(cooldown);
            objPool.GetObject(RandomPointInBounds());
        }
    }

    // Lets a marble tell the spawner it needs to be deleted, without giving marbles access to the pool.
    public void RemoveObject(GameObject marble)
    {
        objPool.ReleaseObject(marble);
    }

    public void DestroyPool(GameObject marble)
    {
        objPool.OnDestroyPooledObject(marble);
    }


    // Get a random point within our collider's bounds.
    private Vector3 RandomPointInBounds()
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
            );
    }
}
