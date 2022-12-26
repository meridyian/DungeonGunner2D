using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    #region Tooltip

    [Tooltip("Populate this array with prefabs that you want to add to the pool and specify the number of gameobjects to be created for each")]

    #endregion

    [SerializeField] private Pool[] poolArray = null;
    private Transform objectpoolTransform;
    private Dictionary<int, Queue<Component>> poolDictionary = new Dictionary<int, Queue<Component>>();


    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
        public string componentType;
    }

    private void Start()
    {
        // this singleton gameobject will be the object pool parent
        objectpoolTransform = this.gameObject.transform;
        
        // create object pools on start
        for (int i = 0; i < poolArray.Length; i++)
        {
            CreatePool(poolArray[i].prefab, poolArray[i].poolSize, poolArray[i].componentType);
            
        }
    }
    
    // create the object pool with the specified prefabs and the specified pool size for each
    private void CreatePool(GameObject prefab, int poolSize, string componentType)
    {
        int poolKey = prefab.GetInstanceID();

        string prefabName = prefab.name;

        GameObject parentGameObject = new GameObject(prefabName + "Anchor"); // create parent  gameobject to parent the child objects to 

        parentGameObject.transform.SetParent(objectpoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<Component>());

            for (int i = 0; i < poolSize; i++)
            {
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                
                newObject.SetActive(false);
                
                poolDictionary[poolKey].Enqueue(newObject.GetComponent(Type.GetType(componentType)));
            }
        }

    }
    
    // Reuse a gameobject component in the pool. "prefab" is the prefab gameobject containing the component. "position" is the world
    // position for the gameobject where it should appear when enabled. "rotation" should be set if the gameobject needs to be rotated.
    public Component ReuseComponent(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            // get object from pool queue
            Component componentToReuse = GetComponentFromPool(poolKey);

            ResetObject(position, rotation, componentToReuse, prefab);

            return componentToReuse;
        }
        else
        {
            Debug.Log("No object pool for" + prefab);
            return null;
        }
    }
    
    
    // get a gameobject component from the pool using the "poolKey"
    private Component GetComponentFromPool(int poolKey)
    {
        Component componentToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(componentToReuse);

        if (componentToReuse.gameObject.active == true)
        {
            componentToReuse.gameObject.SetActive(false);
        }

        return componentToReuse;
    } 
    
    
    // reset the gameobject

    private void ResetObject(Vector3 position, Quaternion rotation, Component componentToReuse, GameObject prefab)
    {
        componentToReuse.transform.position = position;
        componentToReuse.transform.rotation = rotation;
        componentToReuse.gameObject.transform.localScale = prefab.transform.localScale;

    }
    
    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(poolArray), poolArray);
    }
#endif
    #endregion
}
