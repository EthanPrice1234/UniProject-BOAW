using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static GameObject[] persistentObjects = new GameObject[4];
    public int objectIndex = 0;

    void Awake()
    {
        if (persistentObjects[objectIndex] == null)
        {
            persistentObjects[objectIndex] = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else if (persistentObjects[objectIndex] != gameObject)
        {
            Destroy(gameObject);
        }
    }
}
