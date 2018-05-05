using UnityEngine;
using System.Collections;

public class test2 : MonoBehaviour {

    Rigidbody2D rig;
    public float time = 3;
    public float speed = 1;
    float _time = 0;

	// Use this for initialization
	void Start () {
        rig = this.GetComponent<Rigidbody2D>();
        rig.velocity = new Vector2(0, speed);
	}
	
	// Update is called once per frame
	void Update () {
        _time += Time.deltaTime;
        if(_time > time)
        {
            rig.velocity = rig.velocity.y > 0 ? new Vector2(0, -speed) : new Vector2(0, speed);
            _time = 0;
        }
	}
}
