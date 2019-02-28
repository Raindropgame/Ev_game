using UnityEngine;
using System.Collections;

public class LimitFPS : MonoBehaviour {

    private void Awake()
    {
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(this.gameObject);
    }
}
