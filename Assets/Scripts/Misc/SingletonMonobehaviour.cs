using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T: MonoBehaviour
{

    private static T instance;
    // static variables can be accessed using the class name
    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }




}
