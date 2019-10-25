using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool {

    private static Dictionary<GameObject, Queue<GameObject>> _poolAvailable = new Dictionary<GameObject, Queue<GameObject>>();
    private static Dictionary<GameObject, GameObject> _prefabMapper = new Dictionary<GameObject, GameObject>();
    private static Dictionary<Type, Queue<GameObject>> _typePoolAvailable = new Dictionary<Type, Queue<GameObject>>();

    private static int SizeToAddAtFull = 5;
    private static Transform _transformInstance;
    private static Transform transform {
        get {
            if (_transformInstance == null)
                _transformInstance = new GameObject("ObjectPool").transform;

            return _transformInstance;
        }
    }

    private static  GameObject GetObjectFromPool(GameObject objKey)
    {
        if (_poolAvailable[objKey].Count <= 0) return null;
        GameObject obj = _poolAvailable[objKey].Dequeue();
        _prefabMapper[obj] = objKey;
        obj.transform.localScale = objKey.transform.localScale;
        obj.transform.rotation = objKey.transform.rotation;
        obj.transform.position = objKey.transform.position;
        obj.SetActive(true);
        return obj;
    }
    
    //Public facing methods
    public static void Destroy(GameObject obj)
    {
        if (_prefabMapper.ContainsKey(obj))
        {
            _poolAvailable[_prefabMapper[obj]].Enqueue(obj);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
        }
        else
            UnityEngine.Object.Destroy(obj);
    }
    public static void Destroy<T>(GameObject obj)
    {
        if (_typePoolAvailable.ContainsKey(typeof(T)) && obj.GetComponent<T>() != null)
        {
            _typePoolAvailable[typeof(T)].Enqueue(obj);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
        }
        else
            UnityEngine.Object.Destroy(obj);
    }
    
    public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject temp = Instantiate(prefab);
        temp.transform.position = position;
        temp.transform.rotation = rotation;
        if(parent != null)
            temp.transform.SetParent(transform);
        return temp;
    }
    public static GameObject Instantiate(GameObject prefab)
    {
        if (!_poolAvailable.ContainsKey(prefab)) _poolAvailable[prefab] = new Queue<GameObject>();
        if (_poolAvailable[prefab].Count <= 0)
        {
            //Add object to fill out pool
            for (int i = 0; i < SizeToAddAtFull; i++)
            {
                GameObject temp = GameObject.Instantiate(prefab, transform, true);
                temp.name = prefab.name + "(" + i + ")";
                temp.SetActive(false);
                _poolAvailable[prefab].Enqueue(temp);
            }
        }
    
        return GetObjectFromPool(prefab);
    }
    
    public static T Instantiate<T>(Vector3 position, Quaternion rotation, Transform parent = null) where T : Component 
    {
        T temp = Instantiate<T>();
        
        temp.transform.position = position;
        temp.transform.rotation = rotation;
        if(parent != null)
            temp.transform.SetParent(transform);
        return temp;
    }

    public static T Instantiate<T>() where  T : Component 
    {
        Type type = typeof(T);
        if (!_typePoolAvailable.ContainsKey(type)) _typePoolAvailable.Add(type, new Queue<GameObject>());
        
        if (_typePoolAvailable[type].Count == 0)
        {
            //Add object to fill out pool
            for (int i = 0; i < SizeToAddAtFull; i++)
            {
                GameObject temp = new GameObject(type.Name);
                temp.transform.parent = transform;
                temp.AddComponent<T>();
                temp.SetActive(false);
                _typePoolAvailable[type].Enqueue(temp);
            }
        }
        
        GameObject obj = _typePoolAvailable[type].Dequeue();
        obj.SetActive(true);
        return obj.GetComponent<T>();
    }
}
