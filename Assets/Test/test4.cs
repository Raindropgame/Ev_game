using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class test4 : MonoBehaviour {

    public Text text;

	// Use this for initialization
	void Start () {
        StartCoroutine(test());
	}

    IEnumerator test()
    {
        Debug.Log(1);
        yield return new WaitForSeconds(1);
        Debug.Log(2);
        yield return new WaitForSeconds(2);
        Debug.Log(3);
    }
}
