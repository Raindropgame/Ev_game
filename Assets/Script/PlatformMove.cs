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
        rig.velocity = new Vector2(0, this.transform.position.y > targetPosition.y ? -speed : speed);
	}
	
	void Update () {
        if(!t)  //起始到终点
        {
            //this.transform.position = Vector2.MoveTowards(this.transform.position, targetPosition, speed * Time.deltaTime);
            if(Mathf.Abs(this.transform.localPosition.y - targetPosition.y) < 0.5f)
            {
                rig.velocity = new Vector2(0, this.transform.position.y > startPosition.y ? -speed : speed);
                t = !t;
            }
        }
        else   //终点到起始
        {
            //this.transform.position = Vector2.MoveTowards(this.transform.position, startPosition, speed * Time.deltaTime);
            if (Mathf.Abs(this.transform.localPosition.y - startPosition.y) < 0.1f)
            {
                rig.velocity = new Vector2(0, this.transform.position.y > targetPosition.y ? -speed : speed);
                t = !t;
            }
        }
	}
}
