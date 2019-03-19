using UnityEngine;
using System.Collections;

public class Door_2_button : MonoBehaviour {

    public float Force;
    public float PressHeight;
    [HideInInspector]
    public bool isPress = false;

    private Rigidbody2D rig;
    private Vector3 originPos;


    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        originPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (transform.position.y < originPos.y - 0.2f)  //被按下
        {
            rig.velocity = Vector2.up * Force;
        }
        else if (transform.position.y > originPos.y + 0.05f)  //抬起过高
        {
            transform.position = originPos;
            rig.velocity = Vector2.zero;
        }

        if(transform.position.y < originPos.y - PressHeight)
        {
            isPress = true;
        }
        else if(transform.position.y > originPos.y - 0.2f)
        {
            isPress = false;
        }
    }
}
