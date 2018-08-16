using UnityEngine;
using System.Collections;

public class PlatformMove : MonoBehaviour {

    public Vector2 targetPosition;
    public float time;

    private Rigidbody2D rig;
    private bool t = false;
    private Vector2 startPosition;
    private float speed;

	void Start () {
        speed = Mathf.Abs(targetPosition.y - this.transform.position.y) / time;
        targetPosition.x = this.transform.position.x;
        rig = this.GetComponent<Rigidbody2D>();
        if(rig == null)  //如果改物体上没有该组件
        {
            rig = this.gameObject.AddComponent<Rigidbody2D>();
            rig.gravityScale = 0;
            rig.freezeRotation = true;
            rig.isKinematic = false;
        }
        startPosition = this.transform.localPosition;
        rig.velocity = new Vector2(0, this.transform.localPosition.y > targetPosition.y ? -speed : speed);
	}
	
	void Update () {
        if(!t)  //起始到终点
        {
            if((this.transform.localPosition.y < startPosition.y && this.transform.localPosition.y < targetPosition.y ) || (this.transform.localPosition.y > startPosition.y && this.transform.localPosition.y > targetPosition.y))
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
}
