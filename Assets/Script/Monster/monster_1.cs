﻿using UnityEngine;
using System.Collections;

public class monster_1 : Monster_base {

    //来回徘徊
    //不攻击玩家，无伤害，可被玩家攻击
    //在一定范围内会向玩家移动   

	public enum monster_1_state
    {
        walk,
        idle
    }

    public float walkSpeed;
    public Transform leftPoint,rightPoint;
    [HideInInspector]
    public monster_1_state currentState = monster_1_state.walk;
    [HideInInspector]
    public dir Dir = dir.right;
    public GameObject deadParticle;
    [Header("受伤的颜色")]
    [HideInInspector]

    private Rigidbody2D rig;
    private Animator animator;

    override public void onStart()
    {
        rig = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        SR = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        switch(currentState)
        {
            case monster_1_state.idle:

                if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Base.monster_1_idle_2"))
                {
                    if(Random.value < 0.01f)
                    {
                        animator.SetTrigger("idle_2");
                    }
                }

                if (((Vector2)(transform.position - CharacterControl.instance.transform.position)).sqrMagnitude > 1)  //玩家离开变为行走
                {
                    currentState = monster_1_state.walk;
                    animator.SetTrigger("walk");
                }
                break;
            case monster_1_state.walk:

                if( isSeePlayer() )  //感知到玩家
                {
                    rig.velocity = new Vector2(CharacterControl.instance.transform.position.x - transform.position.x > 0 ? walkSpeed : -walkSpeed, rig.velocity.y);  //添加速度
                    Dir = CharacterControl.instance.transform.position.x - transform.position.x > 0 ? dir.right : dir.left;
                }
                else
                {
                    rig.velocity = new Vector2(Dir == dir.left ? -walkSpeed : walkSpeed, rig.velocity.y);  //添加速度
                }

                if ( ((Vector2)(transform.position - CharacterControl.instance.transform.position)).sqrMagnitude < 1)  //在玩家旁边就改变状态为闲置
                {
                    currentState = monster_1_state.idle;
                    animator.SetTrigger("idle_1");
                    rig.velocity = Vector2.zero;
                }
                break;
        }

        transform.localScale = new Vector3(Dir == dir.left ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);  //改变动画方向

        if(isNearEdge() || NearWall() < 0.3f) //临近边或者靠近墙就改变方向
        {
            Dir = Dir == dir.left ? dir.right : dir.left;
        }
    }

    float NearWall()  //检测离墙的距离
    {
        LayerMask layerMask = 1<<9;
        RaycastHit2D HitPoint = Physics2D.Raycast(leftPoint.position, Dir == dir.left ? Vector2.left : Vector2.right, 1f, layerMask);
        if(HitPoint.transform == null)
        {
            return 1000f;
        }
        return HitPoint.distance;
    }

    bool isNearEdge()   //是否在边上
    {
        LayerMask layerMask = 1<<9;
        RaycastHit2D HitPoint = Physics2D.Raycast(leftPoint.position, Vector2.down, 2f, layerMask);
        if(HitPoint.distance <= 0)
        {
            return true;
        }
        return false;
    }

    bool isSeePlayer()  //是否看到了主角
    {
        RaycastHit2D HitPoint = Physics2D.Raycast(rightPoint.position, Dir == dir.right ? Vector2.left : Vector2.right, 7f);
        if (HitPoint.transform != null)
        {
            if (HitPoint.transform.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator die()  //死亡
    {
        this.GetComponent<BoxCollider2D>().enabled = false;
        deadParticle.SetActive(true);
        GetComponent<SpriteRenderer>().enabled = false;
        Time.timeScale = 0;
        StartCoroutine(CameraFollow.instance.shakeCamera(0.25f, 0.04f, 0.2f));  //镜头抖动
        yield return new WaitForSecondsRealtime(0.1f);  //卡屏
        Time.timeScale = 1;
        yield return new WaitForSeconds(deadParticle.GetComponent<ParticleSystem>().startLifetime);
        Destroy(this.gameObject);
    }


    override public void _getHurt(int damage,Attribute attibute)
    {
        currentHP -= damage;
        if (currentHP <= 0)  //是否死亡
        {
            StartCoroutine(die());
            return;
        }
        StartCoroutine(beHurt());
    }


}