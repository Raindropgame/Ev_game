using UnityEngine;
using System.Collections;

public class PlatformMove : MonoBehaviour {

    [Header("0:水平  1：垂直")]
    [Range(0,1)]
    public int horizontal_or_vertical = 1;  //0:水平  1：垂直
    public Vector2 targetPosition;
    public float time;

    private Rigidbody2D rig;
    private bool t = false;
    private Vector2 startPosition;
    private float speed;

	void Start () {
        if (horizontal_or_vertical == 1)
        {
            speed = Mathf.Abs(targetPosition.y - this.transform.localPosition.y) / time;
            targetPosition.x = this.transform.position.x;
            rig = this.GetComponent<Rigidbody2D>();
            if (rig == null)  //如果改物体上没有该组件
            {
                rig = this.gameObject.AddComponent<Rigidbody2D>();
                rig.gravityScale = 0;
                rig.freezeRotation = true;
                rig.isKinematic = false;
            }
            startPosition = this.transform.localPosition;
            rig.velocity = new Vector2(0, this.transform.localPosition.y > targetPosition.y ? -speed : speed);
        }
        else
        {
            speed = Mathf.Abs(targetPosition.x - this.transform.localPosition.x) / time;
            targetPosition.y = this.transform.position.y;
            rig = this.GetComponent<Rigidbody2D>();
            if (rig == null)  //如果改物体上没有该组件
            {
                rig = this.gameObject.AddComponent<Rigidbody2D>();
                rig.gravityScale = 0;
                rig.freezeRotation = true;
                rig.isKinematic = false;
            }
            startPosition = this.transform.localPosition;
            rig.velocity = new Vector2(this.transform.localPosition.x > targetPosition.x ? -speed : speed,0);
        }
	}
	
	void Update () {
        if (horizontal_or_vertical == 1)
        {
            if (!t)  //起始到终点
            {
                if ((this.transform.localPosition.y < startPosition.y && this.transform.localPosition.y < targetPosition.y) || (this.transform.localPosition.y > startPosition.y && this.transform.localPosition.y > targetPosition.y))
                {
                    rig.velocity = new Vector2(0, this.transform.localPosition.y > startPosition.y ? -speed : speed);
                    t = !t;
                }
            }
            else   //终点到起始
            {
                if ((this.transform.localPosition.y < startPosition.y && this.transform.localPosition.y < targetPosition.y) || (this.transform.localPosition.y > startPosition.y && this.transform.localPosition.y > targetPosition.y))
                {
                    rig.velocity = new Vector2(0, this.transform.localPosition.y > targetPosition.y ? -speed : speed);
                    t = !t;
                }
            }
        }
        else
        {
            if (!t)  //起始到终点
            {
                if ((this.transform.localPosition.x < startPosition.x && this.transform.localPosition.x < targetPosition.x) || (this.transform.localPosition.x > startPosition.x && this.transform.localPosition.x > targetPosition.x))
                {
                    rig.velocity = new Vector2(this.transform.localPosition.x > startPosition.x ? -speed : speed, 0);
                    t = !t;
                }
            }
            else   //终点到起始
            {
                if ((this.transform.localPosition.x < startPosition.x && this.transform.localPosition.x < targetPosition.x) || (this.transform.localPosition.x > startPosition.x && this.transform.localPosition.x > targetPosition.x))
                {
                    rig.velocity = new Vector2(this.transform.localPosition.x > targetPosition.x ? -speed : speed, 0);
                    t = !t;
                }
            }
        }
	}
}
