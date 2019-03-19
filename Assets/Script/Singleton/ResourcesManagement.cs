using UnityEngine;
using System.Collections;

public class ResourcesManagement{

    static private ResourcesManagement instance = null;
    static public ResourcesManagement getInstance()
    {
        if(instance == null)
        {
            instance = new ResourcesManagement();
        }
        return instance;
    }

    public T getResources<T>(string name) where T : Object
    {
        T result = default(T);
        result = (T)(Object)Resources.Load<T>(name);
        return result;
    }
}
