using UnityEngine;
using System.Collections;

public class test6 : MonoBehaviour {

    Coroutine t = null;

    private void Start()
    {
        t = StartCoroutine(tt(5));       
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            StopCoroutine(t);
        }
    }

    IEnumerator tt(int a)
    {
        while(true)
        {
            Debug.Log(Time.frameCount + "   " + a);
            yield return null;
        }
    }
}
