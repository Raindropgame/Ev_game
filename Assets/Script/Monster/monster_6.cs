using UnityEngine;
using System.Collections;

public class monster_6 :Monster_base {

    //木属性对其无效
    //火属性使其提升攻击

    public enum monster_6_state
    {
        idle,
        attack,
        walk,
    }

    public float walkSpeed;
    public float attackSpeed;
    public float viewDistance;
    public GameObject deadParticle;
    public Element ElementTrigger;

    private monster_6_state currentState = monster_6_state.walk;
    private Vector3 eye_Left, eye_Right;
    private bool _isNearWall = false;
    private bool _isNearEdge = false;
    private bool _isSeePlayer = false;
    private float m_angryValue = 0;
    private float angryValue
    {
        set
        {
            if(value < 0)
            {
                m_angryValue = 0;
            }
            else
            {
                m_angryValue = value;
            }
        }
        get
        {
            return m_angryValue;
        }
    }
    private bool isDash = false;
    private RaycastHit2D hitPoint;
    private GameObject Attack_Effect;

    public override void onStart()
    {
        base.onStart();

        changeState(currentState);

        changeDir(dir.left);
        eye_Left = colliderID[0].bounds.min - transform.position + GameFunction.getVector3(-0.2f,0.2f,0);
        changeDir(dir.right);
        eye_Right = colliderID[0].bounds.max + Vector3.down * colliderID[0].bounds.size.y - transform.position + GameFunction.getVector3(0.2f, 0.2f, 0);
        Attack_Effect = GameFunction.GetGameObjectInChildrenByName(this.gameObject, "Attack_Effect");
    }

    protected override void _FixedUpdate()
    {
        base._FixedUpdate();

        _isSeePlayer = isSeePlayer();
        _isNearEdge = isNearEdge();
        _isNearWall = isNearWall();

        if(!_isSeePlayer)
        {
            angryValue -= 2 * Time.deltaTime;
        }

        //遇火灼烧
        if (!isBurning)
        {
            if (ElementTrigger.isContainElement(Attribute.fire))
            {
                StartCoroutine(IE_beBurn());
            }
        }

        switch(currentState)
        {
            case monster_6_state.attack:
                
                if(isDash)
                {
                    rig.velocity = (Dir == dir.left ? Vector2.left : Vector2.right) * attackSpeed * Mathf.Lerp(1, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

                    if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
                    {
                        isDash = false;
                        changeState(monster_6_state.idle);
                        angryValue -= 5;
                    }
                }
                break;


            case monster_6_state.idle:

                if(_isSeePlayer)
                {
                    angryValue += 5 * Time.deltaTime;
                }

                if(angryValue > 5)
                {
                    changeState(monster_6_state.attack);
                }
                else
                {
                    if(angryValue < 0.1f)
                    {
                        //在地上变为行走
                        if (isGround())
                        {
                            changeState(monster_6_state.walk);
                        }
                    }
                }
                break;


            case monster_6_state.walk:

                bool _isGround = isGround();
                if (_isGround)
                {
                    rig.velocity = Dir == dir.left ? Vector2.left * walkSpeed : Vector2.right * walkSpeed;
                }
                else
                {
                    changeState(monster_6_state.idle);
                }

                //调头
                if(_isNearEdge || _isNearWall)
                {
                    if (_isGround)
                    {
                        changeDir(Dir == dir.left ? dir.right : dir.left);
                    }
                }


                //看见玩家
                if(_isSeePlayer)
                {
                    angryValue += Time.deltaTime * 2;
                    if (angryValue > 1)
                    {
                        changeState(monster_6_state.idle);
                    }
                }
                break;
        }
    }

    //改变状态
    void changeState(monster_6_state state)
    {
        currentState = state;
        animator.SetTrigger(state.ToString());

        if(state == monster_6_state.attack)
        {
            isDash = false;
        }
        else
        {
            if(state == monster_6_state.idle)
            {
                rig.velocity = Vector2.zero;
            }
        }
    }

    //改变方向
    void changeDir(dir d)
    {
        if (Dir != d)
        {
            Dir = d;
            GameFunction.t_Vector3.Set(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            transform.localScale = GameFunction.t_Vector3;
        }
    }

    bool isSeePlayer()
    {
        int mask = (1 << 17) | (1 << 9);
        hitPoint = Physics2D.Raycast(colliderID[0].bounds.center, Dir == dir.left?Vector2.left:Vector2.right, viewDistance, mask);
        if(hitPoint.transform!=null)
        {
            if(hitPoint.transform.tag.CompareTo("Player") == 0)
            {
                return true;
            }
        }
        return false;
    }

    bool isNearWall()
    {
        int mask = 1 << 9;
        hitPoint = Physics2D.Raycast((Dir == dir.left ? eye_Left : eye_Right) + transform.position, Vector2.up, colliderID[0].bounds.size.y, mask);
        if(hitPoint.transform != null)
        {
            return true;
        }
        return false;
    }

    bool isNearEdge()
    {
        int mask = 1 << 9;
        hitPoint = Physics2D.Raycast((Dir == dir.left ? eye_Left : eye_Right) + transform.position, Vector2.down, 1f, mask);
        if(hitPoint.transform != null)
        {
            return false;
        }
        return true;
    }

    public void startDash()
    {
        isDash = true;
    }

    override public void _getHurt(int damage, Attribute attribute, Vector2 ColliderPos)
    {
        //木属性无效
        if(attribute == Attribute.wood)
        {
            return;
        }

        currentHP -= damage;

        //被攻击有一定几率给予及时反击
        if(Random.value < 0.2f)
        {
            changeState(monster_6_state.attack);
        }

        if (attribute == Attribute.fire)
        {
            if (abnormalState.Contains(AbnormalState.frozen))
            {
                CameraFollow.instance.Stop(0.17f, 0.1f);  //屏幕特效
                StartCoroutine(CameraFollow.instance.shakeCamera(0.25f, 0.04f, 0.2f));  //镜头抖动
                GameObject t = Resources.Load<GameObject>("fire");
                Instantiate(t, position: SR.bounds.center, rotation: Quaternion.Euler(0, 0, 0));
                currentHP -= damage;  //双倍伤害
            }
        }

        base._getHurt(damage, attribute, ColliderPos);
        if (currentHP <= 0)
        {
            return;
        }

        if (attribute == Attribute.ice)  //冰冻
        {
            StartCoroutine(frozen());
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
            if (!abnormalState.Contains(AbnormalState.stone))
            {
                StartCoroutine(electricShock());
            }
        }
        StartCoroutine(beHurt());
    }

    override protected IEnumerator die()  //死亡
    {
        Attack_Effect.SetActive(false);

        this.GetComponent<BoxCollider2D>().enabled = false;
        deadParticle.SetActive(true);
        GetComponent<SpriteRenderer>().enabled = false;
        this.enabled = false;

        Time.timeScale = 0;
        StartCoroutine(CameraFollow.instance.shakeCamera(0.25f, 0.04f, 0.2f));  //镜头抖动
        yield return new WaitForSecondsRealtime(0.1f);  //卡屏
        Time.timeScale = 1;
        yield return new WaitForSeconds(deadParticle.GetComponent<ParticleSystem>().startLifetime);
        Destroy(this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag.CompareTo("Player") == 0)
        {
            CharacterControl.instance.hurt(isBurning?2:1,Attribute.normal,colliderID[0].bounds.center);
        }
    }


    private bool isBurning = false;
    IEnumerator IE_beBurn()
    {
        isBurning = true;
        Attribute originAttr = ElementTrigger.element;
        ElementTrigger.element = Attribute.fire;

        float burnTime = 5;
        float Timer_1 = 0;

        //开始
        while(Timer_1 < burnTime * 0.1f)
        {
            Timer_1 += Time.deltaTime;
            float t = Timer_1 / (burnTime * 0.1f);

            //渐变动画
            SR.color = Color.Lerp(Color.white, GameFunction.getColor(1, 0.48f, 0.48f,1), t);

            yield return null;
        }

        //持续
        Timer_1 = 0;
        while(Timer_1 < burnTime)
        {
            Timer_1 += Time.deltaTime;

            if(ElementTrigger.isContainElement(Attribute.ice) || isInRain)
            {
                break;
            }

            yield return null;
        }

        //结束
        Timer_1 = 0;
        while (Timer_1 < burnTime * 0.1f)
        {
            Timer_1 += Time.deltaTime;
            float t = Timer_1 / (burnTime * 0.1f);

            //渐变动画
            SR.color = Color.Lerp(GameFunction.getColor(1, 0.48f, 0.48f,1), Color.white, t);

            yield return null;
        }
        isBurning = false;
        ElementTrigger.element = originAttr;
    }
}
