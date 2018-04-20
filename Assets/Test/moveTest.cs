using UnityEngine;
using System.Collections;

public class moveTest : MonoBehaviour {

    public float speed = 5;
    private Rigidbody2D rig;

    private void Start()
    {
        rig = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        rig.velocity = new Vector2(0, 0);
        if(Input.GetKey(KeyCode.A))
        {
            rig.velocity += new Vector2(-speed, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rig.velocity += new Vector2(speed, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            rig.velocity += new Vector2(0, speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rig.velocity += new Vector2(0, -speed);
        }
    }
}
