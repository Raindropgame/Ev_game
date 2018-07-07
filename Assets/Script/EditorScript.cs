using UnityEngine;
using System.Collections;

public class EditorScript : MonoBehaviour {

    static EditorScript instance;

    [HideInInspector]
    public bool isEditor = true;

    private void Awake()
    {
        isEditor = false;
    }

	
}
