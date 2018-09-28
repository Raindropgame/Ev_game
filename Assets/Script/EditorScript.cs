using UnityEngine;
using System.Collections;

public class EditorScript : MonoBehaviour {

    [HideInInspector]
    public bool isEditor = true;

    private void Awake()
    {
        isEditor = false;
    }

	
}
