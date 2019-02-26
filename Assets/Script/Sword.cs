using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour {
    //刀


    public float Velocity;
    public float backVelocity_enemy;


    private PolygonCollider2D Collider;
    private Vector2 originPosition;
    private ContactPoint2D HitPoint;
    private Animator animator;
    private Element ElementTrigger = null;

    private void Awake()
    {
        Collider = this.GetComponent<PolygonCollider2D>();
        animator = GetComponent<Animator>();
        originPosition = this.transform.position;

        try
        {
            ElementTrigger = GetComponentInChildren<Element>();
        }
        catch
        {

        }

    }

    private void Update()
    {
        this.transform.localPosition = originPosition;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer.CompareTo(LayerMask.NameToLayer("terrain")) == 0)  //碰到地面
        {
            CharacterControl.instance.add_Velocity(new Vector2(CharacterControl.instance.Dir == dir.left ? Velocity : -Velocity, 0));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.CompareTo("enemy") == 0)  //攻击敌人
        {
            CharacterControl.instance.add_Velocity(new Vector2(CharacterControl.instance.Dir == dir.left ?  backVelocity_enemy: -backVelocity_enemy, 0));  //击退并加特效
            GameObject t = CharacterObjectManager.instance.getHitPoint();
            t.SetActive(true);
            t.transform.position = collision.bounds.center;
            t.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

            CharacterObjectManager.instance.sendHurt(CharacterAttribute.GetInstance().Attack[(int)Arms.swords], CharacterAttribute.GetInstance().ArmsAttribute[(int)Arms.swords], collision.gameObject.GetInstanceID(),Collider.bounds.center);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider.isTrigger = true;  //碰到物体变为触发器
        HitPoint = collision.contacts[0];  //获取碰撞点
        if (collision.gameObject.layer.CompareTo(LayerMask.NameToLayer("terrain")) == 0)
        {
            GameObject t = CharacterObjectManager.instance.getHitPoint();
            t.SetActive(true);
            t.transform.position = HitPoint.point;
            t.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

            CharacterObjectManager.instance.sendHurt(0, CharacterAttribute.GetInstance().ArmsAttribute[(int)Arms.swords], collision.gameObject.GetInstanceID(),Vector2.zero);
        }
    }

    private void OnEnable()
    {
        CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.swords].GemWork();   //结晶作用
        try
        {
            ElementTrigger.element = CharacterAttribute.GetInstance().ArmsAttribute[(int)Arms.swords];
        }
        catch
        {

        }
        animator.SetTrigger(CharacterAttribute.GetInstance().ArmsAttribute[(int)Arms.swords].ToString());   //根据属性更改动画
    }

    private void OnDisable()
    {
        this.transform.localPosition = originPosition;
        Collider.isTrigger = false;
    }

}
