﻿using UnityEngine;
using System.Collections;

public class bullet_monster_2 : MonoBehaviour {

    //monster_2  的子弹

    public float boomTime;   //多少时间后爆炸
    public GameObject BoomEffect;
    public int Damage = 2;

    private CircleCollider2D coll;
    private SpriteRenderer SR;
    private Rigidbody2D rig;

    private void Awake()
    {
        coll = GetComponent<CircleCollider2D>();
        SR = GetComponent<SpriteRenderer>();
        rig = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        coll.enabled = true;
        SR.enabled = true;
        Invoke("boom", boomTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            rig.velocity = Vector2.zero;
            boom();
            CancelInvoke("boom");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && SR.enabled == false)
        {
            CharacterControl.instance.hurt(Damage, Attribute.wood);  //对玩家造成伤害
        }
        if (collision.transform.tag == "enemy" && SR.enabled == false)
        {
            collision.transform.GetComponent<Monster_base>().getHurt(Damage, Attribute.wood, collision.gameObject.GetInstanceID());  //对怪物造成伤害
        }      
    }

    void boom()  //爆炸
    {
        SR.enabled = false;
        coll.enabled = false;
        BoomEffect.SetActive(true);
        Invoke("setFalse", 1);  //播放完特效后关闭
    }

    void setFalse()
    {
        BoomEffect.SetActive(false);
        this.gameObject.SetActive(false);
        CancelInvoke("boom");
    }

}