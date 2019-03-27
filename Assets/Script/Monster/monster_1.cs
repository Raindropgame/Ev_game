using UnityEngine;
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
    public GameObject deadParticle;

    private bool _isSeePlayer = false;
    private bool _isNearEdge = false;
    private float CollHeight;

    public override void onStart()
    {
        base.onStart();

        CollHeight = colliderID[0].bounds.extents.y * 2;
        leftPoint.transform.position = GameFunction.GetGameObjectInChildrenByName(this.gameObject,"Gravity").GetComponent<BoxCollider2D>().bounds.min + new Vector3(Dir == dir.left ? -0.05f : 0.05f, 0.01f, 0);
    }

    protected override void _FixedUpdate()
    {
        base._FixedUpdate();

        _isSeePlayer = isSeePlayer();
        _isNearEdge = isNearEdge();

        switch (currentState)
        {
            case monster_1_state.idle:

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Base.monster_1_idle_2"))
                {
                    if (Random.value < 0.01f)
                    {
                        animator.SetTrigger("idle_2");
                    }
                }

                if (((Vector2)(transform.position - CharacterControl.instance.transform.position)).sqrMagnitude > 1)  //玩家离开变为行走
                {
                    if (!(_isSeePlayer && _isNearEdge))  
                    {
                        currentState = monster_1_state.walk;
                        animator.SetTrigger("walk");
                    }
                }
                break;
            case monster_1_state.walk:

                if (_isSeePlayer)  //感知到玩家
                {
                    rig.velocity = new Vector2(CharacterControl.instance.transform.position.x - transform.position.x > 0 ? walkSpeed : -walkSpeed, rig.velocity.y);  //添加速度
                    Dir = CharacterControl.instance.transform.position.x - transform.position.x > 0 ? dir.right : dir.left;
                }
                else
                {
                    rig.velocity = new Vector2(Dir == dir.left ? -walkSpeed : walkSpeed, rig.velocity.y);  //添加速度
                }

                if (((Vector2)(transform.position - CharacterControl.instance.transform.position)).sqrMagnitude < 1)  //在玩家旁边就改变状态为闲置
                {
                    currentState = monster_1_state.idle;
                    animator.SetTrigger("idle_1");
                    rig.velocity = Vector2.zero;
                }
                if(_isSeePlayer && _isNearEdge)  //当看到玩家在悬崖下变为闲置
                {
                    currentState = monster_1_state.idle;
                    animator.SetTrigger("idle_1");
                    rig.velocity = Vector2.zero;
                }
                break;
        }

        if (isGround())
        {
            if (_isNearEdge || NearWall() < 0.3f) //临近边或者靠近墙就改变方向
            {
                if (!(_isSeePlayer && _isNearEdge))
                {
                    Dir = Dir == dir.left ? dir.right : dir.left;
                }
            }
        }
        transform.localScale = new Vector3(Dir == dir.left ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);  //改变动画方向
    }

    float NearWall()  //检测离墙的距离
    {
        LayerMask layerMask = 1<<9;
        RaycastHit2D HitPoint = Physics2D.Raycast(leftPoint.position, Vector2.up, CollHeight, layerMask);
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
        int mask = (1 << 17) | (1 << 9);  //检测特定层
        RaycastHit2D HitPoint = Physics2D.Raycast(rightPoint.position, Dir == dir.right ? Vector2.left : Vector2.right, 7f, mask);
        RaycastHit2D HitPoint_2 = Physics2D.Raycast(leftPoint.position, Dir == dir.left ? Vector2.left : Vector2.right, 7f, mask);
        if (HitPoint.transform != null)
        {
            if (HitPoint.transform.tag == "Player")
            {
                return true;
            }
        }
        if (HitPoint_2.transform != null)
        {
            if (HitPoint_2.transform.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    override protected IEnumerator die()  //死亡
    {
        this.GetComponent<BoxCollider2D>().enabled = false;
        deadParticle.SetActive(true);
        GetComponent<SpriteRenderer>().enabled = false;
        Time.timeScale = 0;
        CameraFollow.instance.shakeCamera(0.25f, 0.04f, 0.2f);  //镜头抖动
        yield return new WaitForSecondsRealtime(0.1f);  //卡屏
        Time.timeScale = 1;
        yield return new WaitForSeconds(deadParticle.GetComponent<ParticleSystem>().startLifetime);
        Destroy(this.gameObject);
    }


    override public void _getHurt(int damage, Attribute attribute, Vector2 ColliderPos)
    {
        currentHP -= damage;

        if (attribute == Attribute.fire)
        {
            if (abnormalState.Contains(AbnormalState.frozen))
            {
                CameraFollow.instance.Stop(GameData.fire_boom_stopTime,0.1f);  //屏幕特效
                Screen1_render.instance.Wave(this.transform.position, 0.5f);
                CameraFollow.instance.shakeCamera(0.2f, 0.03f, 0.3f);  //镜头抖动
                GameObject t = Resources.Load<GameObject>("fire");
                Instantiate(t, position: SR.bounds.center, rotation: Quaternion.Euler(0, 0, 0));
                currentHP -= damage;  //双倍伤害
            }
        }

        base._getHurt(damage, attribute,ColliderPos);

        if(currentHP <= 0)
        {
            return;
        }

        if (attribute == Attribute.ice)
        {
            StartCoroutine(frozen());
        }
        if (attribute == Attribute.wood)
        {
            StartCoroutine(petrochemical());
        }
        if (attribute == Attribute.fire)
        {
            if (Random.value < GameData.burning_proba)  //计算概率
            {
                if (!abnormalState.Contains(AbnormalState.burning) && !abnormalState.Contains(AbnormalState.stone) && !abnormalState.Contains(AbnormalState.frozen))   //是否可以被灼烧
                {
                    StartCoroutine(burning());  //灼烧
                }
            }
        }
        if (attribute == Attribute.lightning)
        {
            if(!abnormalState.Contains(AbnormalState.stone))
            {
                StartCoroutine(electricShock());
            }
        }
        StartCoroutine(beHurt());
    }


}
