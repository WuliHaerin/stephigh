using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// This script creates the clone of required objects and provide them as required
/// </summary>
/// 

public class ObjectPooling : MonoBehaviour
{

    public static ObjectPooling instance;

    public GameObject deathEffect; //ref to death effect prefabs
    public GameObject diamondPrefab; //ref to diamond prefabs
    public GameObject platform, animSpikePlatformLeft, animSpikePlatformRight;
    public GameObject[] commonPlatform, winterPlatform, grassPlatforms;

    public int count = 5; //total clones of each object to be spawned

    List<GameObject> DeathEffect = new List<GameObject>();
    List<GameObject> Diamond = new List<GameObject>();
    List<GameObject> SpawnPlatform = new List<GameObject>();
    List<GameObject> SpawnCommonPlatform = new List<GameObject>();
    List<GameObject> SpawnWinterPlatform = new List<GameObject>();
    List<GameObject> SpawnGrassPlatform = new List<GameObject>();
    List<GameObject> SpawnAnimSpikePlatformLeft = new List<GameObject>();
    List<GameObject> SpawnAnimSpikePlatformRight = new List<GameObject>();

    void Awake()
    {
        MakeInstance();
        InitializeSpawning();
    }

    void MakeInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Use this for initialization
    void InitializeSpawning()
    {
        //DeathEffect
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(deathEffect);
            obj.transform.parent = gameObject.transform;
            obj.SetActive(false);
            DeathEffect.Add(obj);
        }
        //Diamond
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(diamondPrefab);
            obj.transform.parent = gameObject.transform;
            obj.SetActive(false);
            Diamond.Add(obj);
        }
        //Platform
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(platform);
            obj.transform.parent = gameObject.transform;
            obj.SetActive(false);
            SpawnPlatform.Add(obj);
        }

        //commonPlatform
        for (int i = 0; i < count; i++)
        {
            //each platform is spawn in the array 2 times
            for (int j = 0; j < commonPlatform.Length; j++)
            {
                GameObject obj = Instantiate(commonPlatform[j]);
                obj.transform.parent = gameObject.transform;
                obj.SetActive(false);
                SpawnCommonPlatform.Add(obj);
            }
        }

        //WinterPlatform
        for (int i = 0; i < count; i++)
        {
            //each platform is spawn in the array 2 times
            for (int j = 0; j < winterPlatform.Length; j++)
            {
                GameObject obj = Instantiate(winterPlatform[j]);
                obj.transform.parent = gameObject.transform;
                obj.SetActive(false);
                SpawnWinterPlatform.Add(obj);
            }
        }

        //grassPlatform
        for (int i = 0; i < count; i++)
        {
            //each platform is spawn in the array 2 times
            for (int j = 0; j < grassPlatforms.Length; j++)
            {
                GameObject obj = Instantiate(grassPlatforms[j]);
                obj.transform.parent = gameObject.transform;
                obj.SetActive(false);
                SpawnGrassPlatform.Add(obj);
            }
        }

        //animSpikePlatformLeft
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(animSpikePlatformLeft);
            obj.transform.parent = gameObject.transform;
            obj.SetActive(false);
            SpawnAnimSpikePlatformLeft.Add(obj);
        }

        //animSpikePlatformRight
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(animSpikePlatformRight);
            obj.transform.parent = gameObject.transform;
            obj.SetActive(false);
            SpawnAnimSpikePlatformRight.Add(obj);
        }

    }

    //method which is used to call from other scripts to get the clone object

    //DeathEffect
    public GameObject GetDeathEffect()
    {
        for (int i = 0; i < DeathEffect.Count; i++)
        {
            if (!DeathEffect[i].activeInHierarchy)
            {
                return DeathEffect[i];
            }
        }
        GameObject obj = (GameObject)Instantiate(deathEffect);
        obj.transform.parent = gameObject.transform;
        obj.SetActive(false);
        DeathEffect.Add(obj);
        return obj;
    }

    //Diamond
    public GameObject GetDiamond()
    {
        for (int i = 0; i < Diamond.Count; i++)
        {
            if (!Diamond[i].activeInHierarchy)
            {
                return Diamond[i];
            }
        }
        GameObject obj = (GameObject)Instantiate(diamondPrefab);
        obj.transform.parent = gameObject.transform;
        obj.SetActive(false);
        Diamond.Add(obj);
        return obj;
    }

    //Platform
    public GameObject GetPlatfrom()
    {
        for (int i = 0; i < SpawnPlatform.Count; i++)
        {
            if (!SpawnPlatform[i].activeInHierarchy)
            {
                return SpawnPlatform[i];
            }
        }
        GameObject obj = (GameObject)Instantiate(platform);
        obj.transform.parent = gameObject.transform;
        obj.SetActive(false);
        SpawnPlatform.Add(obj);
        return obj;
    }

    //method which is used to call from other scripts to get the clone object
    //commonPlatforms
    public GameObject GetCommonPlatfrom()
    {
        //this statement checks for the in active object in the hierarcy and retun it
        for (int i = 0; i < SpawnCommonPlatform.Count; i++)
        {
            if (!SpawnCommonPlatform[i].activeInHierarchy)
            {
                return SpawnCommonPlatform[i];
            }
        }
        //if the object are less then more are spawned
        GameObject obj = new GameObject();

        for (int j = 0; j < commonPlatform.Length; j++)
        {
            obj = Instantiate(commonPlatform[j]);
            obj.transform.parent = gameObject.transform;
        }
        obj.SetActive(false);
        SpawnCommonPlatform.Add(obj);
        return obj;
    }

    //winterPlatforms
    public GameObject GetWinterPlatfrom()
    {
        //this statement checks for the in active object in the hierarcy and retun it
        for (int i = 0; i < SpawnWinterPlatform.Count; i++)
        {
            if (!SpawnWinterPlatform[i].activeInHierarchy)
            {
                return SpawnWinterPlatform[i];
            }
        }
        //if the object are less then more are spawned
        GameObject obj = new GameObject();

        for (int j = 0; j < winterPlatform.Length; j++)
        {
            obj = Instantiate(winterPlatform[j]);
            obj.transform.parent = gameObject.transform;
        }
        obj.SetActive(false);
        SpawnWinterPlatform.Add(obj);
        return obj;
    }

    //grassPlatforms
    public GameObject GetGrassPlatfrom()
    {
        //this statement checks for the in active object in the hierarcy and retun it
        for (int i = 0; i < SpawnGrassPlatform.Count; i++)
        {
            if (!SpawnGrassPlatform[i].activeInHierarchy)
            {
                return SpawnGrassPlatform[i];
            }
        }
        //if the object are less then more are spawned
        GameObject obj = new GameObject();

        for (int j = 0; j < grassPlatforms.Length; j++)
        {
            obj = Instantiate(grassPlatforms[j]);
            obj.transform.parent = gameObject.transform;
        }
        obj.SetActive(false);
        SpawnGrassPlatform.Add(obj);
        return obj;
    }

    //animSpikePlatformsLeft
    public GameObject GetAnimSpikePlatfromLeft()
    {
        for (int i = 0; i < SpawnAnimSpikePlatformLeft.Count; i++)
        {
            if (!SpawnAnimSpikePlatformLeft[i].activeInHierarchy)
            {
                return SpawnAnimSpikePlatformLeft[i];
            }
        }
        GameObject obj = (GameObject)Instantiate(animSpikePlatformLeft);
        obj.transform.parent = gameObject.transform;
        obj.SetActive(false);
        SpawnAnimSpikePlatformLeft.Add(obj);
        return obj;
    }

    //animSpikePlatformsRight
    public GameObject GetAnimSpikePlatfromRight()
    {
        for (int i = 0; i < SpawnAnimSpikePlatformRight.Count; i++)
        {
            if (!SpawnAnimSpikePlatformRight[i].activeInHierarchy)
            {
                return SpawnAnimSpikePlatformRight[i];
            }
        }
        GameObject obj = (GameObject)Instantiate(animSpikePlatformRight);
        obj.transform.parent = gameObject.transform;
        obj.SetActive(false);
        SpawnAnimSpikePlatformRight.Add(obj);
        return obj;
    }

}