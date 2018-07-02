using UnityEngine;
using System.Collections;

public class platform_jump : MonoBehaviour {

    private bool Rest = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !Rest)
        {
            CharacterControl.instance._jumpTimes--;
        }
    }
}
