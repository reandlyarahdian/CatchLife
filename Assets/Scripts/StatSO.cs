using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Stat
{
    public int health;
    public int point;
}

[Serializable]
public class ObjectsToSpawn
{
    public GameObject prefab;
    public Material material;
}

public enum Power
{
    Jump,
    Walk,
    No
}

[CreateAssetMenu(fileName = "StatSO", menuName = "Character Stats")]
public class StatSO : ScriptableObject
{
    public Stat stat;

    public bool isObject;

    public ObjectsToSpawn toSpawn;

    public Power power;

    public int TurnUps;

    public int health
    {
        get
        {
            return stat.health;
        }
        set
        {
            if (value != stat.health)
            {
                stat.health = Math.Max(value, 0);
                HealthChanged?.Invoke(stat.health);
            }
        }
    }

    public int point
    {
        get
        {
            return stat.point;
        }
        set
        {
            if (value != stat.point)
            {
                stat.point = Math.Max(value, 0);
                PointChanged?.Invoke(stat.point);
            }
        }
    }

    public UnityAction<int> HealthChanged;
    public UnityAction<int> PointChanged;

    public void InitDef()
    {
        if (!isObject)
        {
            if (health != 3)
            {
                stat.health = 3;
                stat.point = 0;
            }
            health = stat.health;
            point = stat.point;
        }
    }

    public GameObject CharacterToSpawn()
    {
        var obj = toSpawn.prefab;
        var skins = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(SkinnedMeshRenderer skin in skins)
            skin.material = toSpawn.material;
        obj.GetComponent<CharacterFall>().Stat(stat, power, TurnUps);
        return obj;
    }
}
