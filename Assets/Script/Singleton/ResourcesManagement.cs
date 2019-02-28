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

    public Object getResources(string name)
    {
        return Resources.Load<GameObject>(name);
    }
}
