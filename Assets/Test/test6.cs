using UnityEngine;
using System.Collections;

public class test6 : MonoBehaviour {

    SpriteRenderer sr;

	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();
        Debug.Log(sr.bounds);
	}

    private void Update()
    {
        Debug.Log(sr.sprite.border);
    }

}
