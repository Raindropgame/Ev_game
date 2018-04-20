using UnityEngine;
using System.Collections;

public class test2 : MonoBehaviour {

    Rigidbody2D rig;
    public float time = 3;
    float _time = 0;

	// Use this for initialization
	void Start () {
        rig = this.GetComponent<Rigidbody2D>();
        rig.velocity = new Vector2(0, 3);
	}
	
	// Update is called once per frame
	void Update () {
        _time += Time.deltaTime;
        if(_time > time)
        {
            rig.velocity = rig.velocity.y > 0 ? new Vector2(0, -3) : new Vector2(0, 3);
            _time = 0;
        }
	}
}
