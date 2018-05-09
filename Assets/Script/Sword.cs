using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour {
    //刀

    public float Velocity;
    public GameObject a;

    private PolygonCollider2D Collider;
    private Vector2 originPosition;
    private ContactPoint2D HitPoint;

    private void Awake()
    {
        Collider = this.GetComponent<PolygonCollider2D>();
        originPosition = this.transform.position;
    }

    private void Update()
    {
        this.transform.localPosition = originPosition;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "map")  //碰到地面
        {
            CharacterControl.instance.add_Velocity(new Vector2(CharacterControl.instance.Dir == dir.left ? Velocity : -Velocity, 0));
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider.isTrigger = true;  //碰到物体变为触发器
        HitPoint = collision.contacts[0];  //获取碰撞点
        if (collision.transform.tag == "map")
        {
            Instantiate(a, position: HitPoint.point, rotation: Quaternion.Euler(0, 0, Random.Range(0, 360)));
        }
    }

    private void OnDisable()
    {
        this.transform.localPosition = originPosition;
        Collider.isTrigger = false;
    }

}
