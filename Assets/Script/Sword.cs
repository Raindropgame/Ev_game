using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour {
    //刀


    public float Velocity;
    public GameObject a;

    private PolygonCollider2D Collider;
    private Vector2 originPosition;
    private ContactPoint2D HitPoint;
    private Animator animator;

    private void Awake()
    {
        Collider = this.GetComponent<PolygonCollider2D>();
        animator = GetComponent<Animator>();
        originPosition = this.transform.position;
    }

    private void Update()
    {
        this.transform.localPosition = originPosition;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "maps")  //碰到地面
        {
            CharacterControl.instance.add_Velocity(new Vector2(CharacterControl.instance.Dir == dir.left ? Velocity : -Velocity, 0));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "enemy")  //攻击敌人
        {
            CharacterObjectManager.instance.sendHurt(CharacterAttribute.GetInstance().jumpArrowAttack, Attribute.normal, collision.gameObject.GetInstanceID());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider.isTrigger = true;  //碰到物体变为触发器
        HitPoint = collision.contacts[0];  //获取碰撞点
        if (collision.transform.tag == "maps")
        {
            GameObject t = CharacterObjectManager.instance.getHitPoint();
            t.SetActive(true);
            t.transform.position = HitPoint.point;
            t.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }
    }

    private void OnEnable()
    {
        animator.SetTrigger(CharacterAttribute.GetInstance().swordAttribte.ToString());   //根据属性更改动画
    }

    private void OnDisable()
    {
        this.transform.localPosition = originPosition;
        Collider.isTrigger = false;
    }

}
