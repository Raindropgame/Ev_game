using UnityEngine;
using System.Collections;

public class test5 : MonoBehaviour {

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
