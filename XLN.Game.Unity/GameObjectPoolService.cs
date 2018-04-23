using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XLN.Game.Common;
using System.Runtime.InteropServices;

[GuidAttribute("1FB5C7FA-6F22-4A51-AB67-57316134D495")]
public class GameObjectPoolService : IService
{

 
    public class PoolObjectModel
    {

        public GameObject Prefab;
        public string PrefabName;
        public ArrayList All;
        public Stack Available;
        
    }

    private Dictionary<string, PoolObjectModel> m_PoolObjectList = new Dictionary<string, PoolObjectModel>();   
    public PoolObjectModel AddNewPoolObject(GameObject prefab, uint initialCapacity)
    {
        //Debug.Log("Add Old object" + prefab.name);
        PoolObjectModel newPoolObject = new PoolObjectModel();
        newPoolObject.Prefab = prefab;
        newPoolObject.PrefabName = prefab.name + "(Cached)";
        newPoolObject.All = (initialCapacity > 0) ? new ArrayList((int) initialCapacity) : new ArrayList();
        newPoolObject.Available = (initialCapacity > 0) ? new Stack((int) initialCapacity) : new Stack();
        m_PoolObjectList.Add(newPoolObject.PrefabName, newPoolObject);

        return newPoolObject;
        
    }

     

    public int NumActive (GameObject prefab)
    {
        PoolObjectModel poolObject = GetPoolObject(prefab);
        if(poolObject == null)
            return 0;
        else
            return (poolObject.All.Count - poolObject.Available.Count);

    }

     

    public int NumAvailable (GameObject prefab)
    {
        PoolObjectModel poolObject = GetPoolObject(prefab);
        if(poolObject == null)
            return 0;
        else
            return poolObject.Available.Count;

    }

     

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {

        GameObject result;
        PoolObjectModel currentPoolObject = new PoolObjectModel();

        if(IsPoolObjectExist(prefab))
        {
            currentPoolObject = GetPoolObject(prefab);
        }
        else
        {
            currentPoolObject = AddNewPoolObject(prefab, 4);
       
        }

        if (currentPoolObject.Available.Count == 0)
        {

            //Debug.Log("Spawn Instantiate new object" + currentPoolObject.Prefab.name);
            result = GameObject.Instantiate(prefab, position, rotation) as GameObject;
            result.name = currentPoolObject.PrefabName;
            GameObject.DontDestroyOnLoad(result);
            //Debug.Log("Spawn Instantiate new object" + result.name);
            currentPoolObject.All.Add(result);
        }
        else
        {
            //Debug.LogError("Pool Has Bug!" + currentPoolObject.Available.ToString() + currentPoolObject.PrefabName);
            //Debug.Log("Reuse Old object" + currentPoolObject.PrefabName);
            result = currentPoolObject.Available.Pop() as GameObject;
            // get the result’s transform and reuse for efficiency.
            // calling gameObject.transform is expensive.

            if(result == null)
            {
                Debug.LogError("Pool Has Bug!" + currentPoolObject.PrefabName);
                //result = GameObject.Instantiate(prefab, position, rotation) as GameObject;
                //result.name = currentPoolObject.Prefab.name;
            }

            Transform resultTrans = result.transform;
            resultTrans.position = position;
            resultTrans.rotation = rotation;

            SetActive(result, true);
            //Debug.Log(“Is Active: ” + result.active);

        }
        return result;

    }

    public GameObject Spawn(GameObject prefab)
    {
        return Spawn(prefab, Vector3.zero, Quaternion.identity);
    }

    public override bool OnDestroy()
    {
        Clear();
        return true;
    }
     

    public bool Destroy(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError("Destroy null");
            return false;
        }
        PoolObjectModel currentPoolObject = GetPoolObject(target);
        if(IsPoolObjectExist(target))
        {
            if (!currentPoolObject.Available.Contains(target))
            {   
                currentPoolObject.Available.Push(target);
                SetActive(target, false);
                return true;
            }

        }
        else
        {
            Debug.LogError("Direct destroy " + target.name);
            GameObject.Destroy(target);
            return true;

        }
        return false;

    }

     

    public void DestroyAll()
    {

        foreach(KeyValuePair<string, PoolObjectModel> kv in m_PoolObjectList)
        {

            for (int i=0; i<kv.Value.All.Count; i++)
            {
                GameObject target = kv.Value.All[i] as GameObject;
                //if (target.active)
                GameObject.Destroy(target);
            }

        }

    }

     

    // Unspawns all the game objects and clears the pool.

    public void Clear() 
    {

        DestroyAll();

        foreach(KeyValuePair < string, PoolObjectModel> kv in m_PoolObjectList)
        {
            kv.Value.Available.Clear();
            kv.Value.All.Clear();

        }
        m_PoolObjectList.Clear();

    }

     

    public bool IsPoolObjectExist(GameObject prefabObject)
    {

        return IsPoolObjectExist(prefabObject.name);

    }

    public bool IsPoolObjectExist(string prefabName)
    {

        bool prefabExists = false;

        if (!prefabName.Contains("(Cached)"))
            prefabName = prefabName + "(Cached)";

        if (m_PoolObjectList.ContainsKey(prefabName))
        {
            prefabExists = true;
        }

        return prefabExists;

    }


     

    protected void SetActive(GameObject target, bool active)
    {
        //Debug.Log("Set Active" + target.name + active);
        DeepActivate(target.transform, active);

    }

    private PoolObjectModel GetPoolObject(string prefabName)
    {
        if (!prefabName.Contains("(Cached)"))
            prefabName = prefabName + "(Cached)";
        PoolObjectModel poolObjectToReturn = null;// new PoolObjectModel();

        m_PoolObjectList.TryGetValue(prefabName, out poolObjectToReturn);
        return poolObjectToReturn;
    }
     

    private PoolObjectModel GetPoolObject(GameObject prefabObject)
    {

        return GetPoolObject(prefabObject.name);

    }

     

    protected void DeepActivate(Transform gameObjectTransform, bool activate)
    {

        foreach (Transform t in gameObjectTransform)
        {
            DeepActivate(t, activate);
        }
        gameObjectTransform.gameObject.SetActive(activate);

    }

}