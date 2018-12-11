using UnityEngine;
using System.Collections;

public class monster_5 : Monster_base{

    public enum monster_5_state
    {
        idle,
        dash,
        stop,
        rest,
    }

    public float dashSpeed;
    public float rest_view_radius;
    public float stopTime;
    public GameObject deadParticle;
    public GameObject eye;

    private monster_5_state currentState = monster_5_state.rest;
    private BoxCollider2D Gravity;
    private Vector3 offset_Left, offset_Right;
    private bool _isSeePlayer = false;
    private bool _isNearWall = false;
    private bool _isNearEdge = false;
    private float m_angryValue = 0;
    private float angryValue
    {
        get
        {
            return m_angryValue;
        }
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
    }
    private float Timer_stop = 0;
    private ParticleSystem.EmissionModule dashParticle;
    private float originParticleRate;

    public override void onStart()
    {
        base.onStart();

        animator.SetTrigger(currentState.ToString());
        Gravity = GameFunction.GetGameObjectInChildrenByName(this.gameObject, "Gravity").GetComponent<BoxCollider2D>();
        dashParticle = GameFunction.GetGameObjectInChildrenByName(this.gameObject, "DashDustParticle").GetComponent<ParticleSystem>().emission;
        originParticleRate = dashParticle.rate.constant;
        dashParticle.rate = 0;

        if (transform.localScale.x < 0)  //向右
        {
            offset_Right = Gravity.bounds.min + Gravity.bounds.size.x * Vector3.right - transform.position + Vector3.one * 0.3f;
            GameFunction.t_Vector3.Set(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            transform.localScale = GameFunction.t_Vector3;
            offset_Left = Gravity.bounds.min - transform.position + new Vector3(-0.3f, 0.3f, 0);
            GameFunction.t_Vector3.Set(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            transform.localScale = GameFunction.t_Vector3;
        }
        else
        {
            offset_Left = Gravity.bounds.min - transform.position + new Vector3(-0.3f, 0.3f, 0);
            GameFunction.t_Vector3.Set(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            transform.localScale = GameFunction.t_Vector3;
            offset_Right = Gravity.bounds.min + Gravity.bounds.size.x * Vector3.right - transform.position + Vector3.one * 0.3f;
            GameFunction.t_Vector3.Set(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            transform.localScale = GameFunction.t_Vector3;
        }

        Dir = (transform.localScale.x < 0 ? dir.right : dir.left);
    }

    protected override void _FixedUpdate()
    {
        base._FixedUpdate();

        if (currentState != monster_5_state.rest)
        {
            _isSeePlayer = isSeePlayer();
        }
        else
        {
            _isSeePlayer = isSeePlayer_rest();
        }
        _isNearEdge = isNearEdge();
        _isNearWall = isNearWall();

        angryValue -= (Time.deltaTime * 1);
        switch (currentState)
        {
            case monster_5_state.idle:

                if(_isSeePlayer)
                {
                    angryValue += (Time.deltaTime * 3.5f);
                }

                if(angryValue > 5)
                {
                    if (_isSeePlayer)
                    {
                        changeState(monster_5_state.dash);
                    }
                }

                if(Weather.instance.getDayState() == DayOrNight.Night && angryValue < 0.1f)
                {
                    if (!_isSeePlayer)
                    {
                        changeState(monster_5_state.rest);
                    }
                }
                break;


            case monster_5_state.dash:
                rig.velocity = (Dir == dir.left ? Vector2.left : Vector2.right) * dashSpeed;

                if(_isNearEdge || _isNearWall)
                {
                    angryValue -= 4;
                    changeState(monster_5_state.stop);
                    Timer_stop = 0;
                }
                break;


            case monster_5_state.rest:

                if (Weather.instance.getDayState() == DayOrNight.Day)
                {
                    if (_isSeePlayer)
                    {
                        angryValue += 10;
                        changeState(monster_5_state.idle);
                        if (CharacterControl.instance.transform.position.x > transform.position.x)
                        {
                            changeDir(dir.right);
                        }
                        else
                        {
                            changeDir(dir.left);
                        }
                    }
                }
                else
                {

                }
                break;


            case monster_5_state.stop:
                Timer_stop += Time.deltaTime;
                float t = Timer_stop / stopTime;
                rig.velocity = Vector2.Lerp(((Dir == dir.left ? Vector2.left : Vector2.right) * dashSpeed), Vector2.zero, t);

                if(Timer_stop > stopTime)
                {
                    Timer_stop = 0;
                    changeState(monster_5_state.idle);
                    changeDir((Dir == dir.left ? dir.right : dir.left));
                }
                break;
        }

        
    }

    //改变状态
    void changeState(monster_5_state state)
    {
        currentState = state;
        animator.SetTrigger(state.ToString());

        //尘土粒子特效
        if(state == monster_5_state.dash || state == monster_5_state.stop)
        {
            dashParticle.rate = originParticleRate;
        }
        else
        {
            dashParticle.rate = 0;
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

    bool isSeePlayer()  //是否看到了主角
    {
        int mask = (1 << 17) | (1 << 9);  //检测特定层
        RaycastHit2D HitPoint = Physics2D.Raycast(transform.position + (Dir == dir.left?offset_Left:offset_Right), Dir == dir.right ? Vector2.right : Vector2.left, 40f, mask);
        if (HitPoint.transform != null)
        {
            if (HitPoint.transform.tag.CompareTo("Player") == 0)
            {
                return true;
            }
        }
        return false;
    }

    bool isNearEdge()   //是否在边上
    {
        LayerMask layerMask = 1 << 9;
        RaycastHit2D HitPoint;
        if (currentState != monster_5_state.dash)
        {
            HitPoint = Physics2D.Raycast(transform.position + (Dir == dir.left ? offset_Left : offset_Right), Vector2.down, 1f, layerMask);
        }
        else
        {
            float edge_distance = 4f;
            HitPoint = Physics2D.Raycast(transform.position + (Dir == dir.left ? offset_Left + Vector3.left * edge_distance : offset_Right + Vector3.right * edge_distance), Vector2.down, 1f, layerMask);

        }
        if (HitPoint.transform == null)
        {
            return true;
        }
        return false;
    }

    bool isNearWall()  //检测离墙的距离
    {
        LayerMask layerMask = 1 << 9;
        RaycastHit2D HitPoint;
        if (currentState != monster_5_state.dash)
        {
            HitPoint = Physics2D.Raycast(transform.position + (Dir == dir.left ? offset_Left : offset_Right), Vector2.up, Gravity.bounds.size.y, layerMask);
        }
        else
        {
            float edge_distance = 4f;
            HitPoint = Physics2D.Raycast(transform.position + (Dir == dir.left ? offset_Left + Vector3.left * edge_distance: offset_Right + Vector3.right * edge_distance), Vector2.up, Gravity.bounds.size.y, layerMask);

        }
        if (HitPoint.transform == null)
        {
            return false;
        }
        return true;
    }

    bool isSeePlayer_rest()
    {
        int mask = (1 << 17) | (1 << 9);  //检测特定层
        RaycastHit2D[] hitPoint = Physics2D.CircleCastAll(transform.position, rest_view_radius, Vector2.zero,mask);
        for(int i = 0;i<hitPoint.Length;i++)
        {
            if(hitPoint[i].transform.tag.CompareTo("Player") == 0)
            {
                return true;
            }
        }
        return false;
    }

    override public void _getHurt(int damage, Attribute attribute, Vector2 ColliderPos)
    {
        currentHP -= damage;

        if(currentState == monster_5_state.rest)
        {
            changeState(monster_5_state.idle);
        }
        else
        {
            if(currentState == monster_5_state.idle)  //被攻击并且没看到人
            {
                if(!_isSeePlayer)
                {
                    changeState(monster_5_state.dash);
                }
            }
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
            if (!abnormalState.Contains(AbnormalState.stone))
            {
                StartCoroutine(electricShock());
            }
        }
        StartCoroutine(beHurt());
    }

    override protected IEnumerator die()  //死亡
    {
        dashParticle.enabled = false;
        eye.SetActive(false);
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
        if (collision.tag.CompareTo("Player") == 0 && isHurtPlayer)
        {
            CharacterControl.instance.hurt(1, Attribute.normal,colliderID[0].bounds.center);
        }
    }

    protected override void OnPetrochemicalStart()
    {
        base.OnPetrochemicalStart();

        //停止发射粒子
        dashParticle.rate = 0;
    }

    protected override void OnPetrochemicalEnd()
    {
        base.OnPetrochemicalEnd();

        //粒子
        if (currentState == monster_5_state.dash || currentState == monster_5_state.stop)
        {
            dashParticle.rate = originParticleRate;
        }
        else
        {
            dashParticle.rate = 0;
        }
    }

    protected override void OnFrozenStart()
    {
        base.OnPetrochemicalStart();

        //停止发射粒子
        dashParticle.rate = 0;
    }

    protected override void OnFrozenEnd()
    {
        base.OnPetrochemicalEnd();

        //粒子
        if (currentState == monster_5_state.dash || currentState == monster_5_state.stop)
        {
            dashParticle.rate = originParticleRate;
        }
        else
        {
            dashParticle.rate = 0;
        }
    }
}
