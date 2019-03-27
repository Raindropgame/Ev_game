using UnityEngine;
using System.Collections;

public class monster_8 : Monster_base{

    /// <summary>
    /// 游荡
    /// 看见玩家使用光束攻击
    /// 淋雨闭合
    /// 不受任何异常状态
    /// </summary>

    enum state_monster_8
    {
        idle,
        attack,
        rest,
    }

    [Header("Self Attribute")]
    public int damage;
    public float speed;
    public float m_view_Player;
    public float Frequency_move;
    public float distance_nearWall = 2;
    public LineRenderer LR;
    public float startWidth_laser;
    public EdgeCollider2D edgeColl;
    public float rate_reduce,rate_seePlayer,rate_attack;
    public float odds_dash;
    public float dashSpeed;
    public Transform Trans_eye,Trans_eyeCenter;
    public SpriteRenderer SR_heart;
    public Color Color_laser;
    public Color restColor_center;
    public SpriteRenderer SR_center;
    public GameObject Particle_Die;

    private state_monster_8 currentState = state_monster_8.idle;
    private bool _isSeePlayer = false;
    private bool _isNearWall = false;
    private Vector2 targetPos;
    private float timer_0 = 0;
    private float time_ignoreWall = 1.5f;
    private bool isAttackFinish = false;
    private bool isLaser = false;
    private Vector2[] edgePos = new Vector2[2];
    private float m_Value_behavor = 0;
    private float Value_behavor
    {
        get
        {
            return m_Value_behavor;
        }
        set
        {
            if(value < 0)
            {
                m_Value_behavor = 0;
            }
            else
            {
                m_Value_behavor = value;
            }
        }
    }
    private Vector3 originScale;
    private Color originColor_heart;
    private Color originColor_center;
    private static ParticleSystem m_Particle_hurt = null;
    private static ParticleSystem Particle_hurt
    {
        get
        {
            if(m_Particle_hurt == null)
            {
                GameObject t = Instantiate(ResourcesManagement.getInstance().getResources<GameObject>("Particle_hurt"), position: Vector3.zero, rotation: Quaternion.Euler(Vector3.zero)) as GameObject;
                m_Particle_hurt = t.GetComponent<ParticleSystem>();
            }
            return m_Particle_hurt;
        }
    }  //受伤粒子动画
    private float view_Player
    {
        get
        {
            if(Weather.instance.getDayState() == DayOrNight.Night)
            {
                return m_view_Player * 0.25f;
            }
            return m_view_Player;
        }
    }

    public override void onStart()
    {
        base.onStart();

        targetPos = getRandomPoint();
        edgePos[0] = Vector2.zero;
        edgePos[1] = Vector2.zero;
        originScale = transform.localScale;
        originColor_heart = SR_heart.color;
        originColor_center = SR_center.color;
    }

    protected override void _FixedUpdate()
    {
        base._FixedUpdate();

        _isSeePlayer = isSeePlayer();
        _isNearWall = isNearWall();
        timer_0 += Time.deltaTime;

        //雨中休息
        if(isInRain)
        {
            changeState(state_monster_8.rest);
        }
        switch (currentState)
        {
            case state_monster_8.idle:
                EyeFollowPlayer(transform.position);
                walk(targetPos);
                Value_behavor -= (Time.deltaTime * rate_reduce);
                //碰到地形  改变方向
                if (_isNearWall && timer_0 > time_ignoreWall)
                {
                    targetPos = getRandomPoint();
                    timer_0 = 0;
                }

                if(_isSeePlayer)
                {
                    Value_behavor += (Time.deltaTime * rate_seePlayer);
                    if(Value_behavor > 10)
                    {
                        //进入攻击模式
                        changeState(state_monster_8.attack);
                    }
                }
                break;
            case state_monster_8.attack:

                EyeFollowPlayer(CharacterControl.instance.transform.position);

                Value_behavor += (Time.deltaTime * rate_attack);
                if(Value_behavor > 13)
                {
                    //一定几率使用冲刺攻击
                    if (Random.value < odds_dash)
                    {
                        StartCoroutine(IE_dash(CharacterControl.instance.transform.position));
                        Value_behavor -= 10;
                    }
                    else if(!isLaser)
                    {
                        isLaser = true;
                        StartCoroutine(IE_Laser());
                        Value_behavor -= 13;
                    }
                }

                if(isAttackFinish)
                {
                    changeState(state_monster_8.idle);
                }
                break;
            case state_monster_8.rest:
                if(!isInRain)
                {
                    changeState(state_monster_8.idle);
                    SR_center.color = originColor_center;
                }
                break;
        }

    }

    public override void _getHurt(int damage, Attribute attribute, Vector2 ColliderPos)
    {
        currentHP -= damage;
        PlayHurtParticle();

        base._getHurt(damage, attribute, ColliderPos);
    }

    protected override IEnumerator die()
    {
        yield return null;
        colliderID[0].enabled = false;
        Instantiate(Particle_Die, position: transform.position, rotation: Quaternion.Euler(Vector3.zero));

        this.gameObject.SetActive(false);

        CameraFollow.instance.shakeCamera(0.25f, 0.04f, 0.2f);  //镜头抖动
        CameraFollow.instance.Stutter(0.1f);  //卡屏
        Destroy(this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag.CompareTo("Player") == 0)
        {
            //身体伤害
            CharacterControl.instance.hurt(damage, Attribute.normal, gravity.bounds.center);
        }
    }

    /// <summary>
    /// 是否看见玩家了
    /// </summary>
    /// <returns></returns>
    bool isSeePlayer()
    {
        Vector2 Vector_Player = ((Vector2)CharacterControl.instance.transform.position - (Vector2)transform.position);
        if(Vector_Player.sqrMagnitude < Mathf.Pow(view_Player,2))
        {
            int mask = (1 << LayerMask.NameToLayer("terrain")) | (1 << LayerMask.NameToLayer("Player"));
            hitPoint = Physics2D.Raycast(transform.position, Vector_Player.normalized, 999, mask);
            if(hitPoint.transform != null)
            {
                if(hitPoint.transform.tag.CompareTo("Player") == 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 向目的点移动
    /// </summary>
    /// <param name="_targetPos"></param>
    void walk(Vector2 _targetPos)
    {
        Vector2 direction =  (_targetPos - (Vector2)transform.position).normalized;
        float angle = Vector2.Angle(Vector2.right, direction);
        if(_targetPos.y < transform.position.y)
        {
            angle = 360 - angle;
        }
        float currentAngle = (Mathf.PingPong(Time.time * Frequency_move, 90) - 45 + angle) * Mathf.Deg2Rad;
        Vector2 currentSpeed = GameFunction.getVector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)) * speed;

        rig.velocity = currentSpeed;
    }


    /// <summary>
    /// 选取一个随机位置
    /// </summary>
    /// <returns></returns>
    Vector2 getRandomPoint()
    {
        Vector2 vector_nearWall = getNearestWallDir();
        float angle = Vector2.Angle(Vector2.right, vector_nearWall.normalized) + 180 + Mathf.Lerp(-1,1,Random.value) * 90;  //选取方向的角度
        Vector2 direction = GameFunction.getVector2(Mathf.Cos(angle), Mathf.Sin(angle));
        int mask = 1 << LayerMask.NameToLayer("terrain");
        for(int i = 0;i<5;i++)
        {
            hitPoint = Physics2D.Raycast(transform.position, direction, 999, mask);
            if(hitPoint.transform != null)
            {
                return hitPoint.point;
            }
        }
        return transform.position;
    }

    Vector2[] dirList = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
    /// <summary>
    /// 寻找就近的墙
    /// </summary>
    /// <returns></returns>
    Vector2 getNearestWallDir()
    {
        int mask = 1 << LayerMask.NameToLayer("terrain");
        float nearestDistance = 999;
        int index = 0;
        for(int i = 0;i<4;i++)
        {
            hitPoint = Physics2D.Raycast(transform.position, dirList[i], 999, mask);
            if(hitPoint.transform != null)
            {
                float t = (hitPoint.point - (Vector2)transform.position).sqrMagnitude;
                if (t < nearestDistance)
                {
                    nearestDistance = t;
                    index = i;
                }
            }
        }

        return dirList[index] * Mathf.Sqrt(nearestDistance);
    }

    void changeState(state_monster_8 state)
    {
        if (currentState != state)
        {
            rig.velocity = Vector2.zero;
            if (state == state_monster_8.rest)
            {
                SR_center.color = restColor_center;
            }
            else if(state == state_monster_8.idle)
            {
                
            }
            else if(state == state_monster_8.attack)
            {
                isAttackFinish = false;
            }
            currentState = state;
        }
    }
    
    bool isNearWall()
    {
        Vector2 t = getNearestWallDir();
        if(t.sqrMagnitude < Mathf.Pow(distance_nearWall,2))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 激光动画
    /// </summary>
    /// <returns></returns>
    IEnumerator IE_Laser()
    {
        int mask = 1 << LayerMask.NameToLayer("terrain");
        float duration = 0.3f;
        float Timer = 0;

        //准备
        const float time_ready = 1f;
        while(Timer < time_ready)
        {
            Timer += Time.deltaTime;
            float t = Timer / time_ready;

            SR_heart.color = Color.Lerp(originColor_heart, Color_laser, GameFunction.smoothLerp(t));

            yield return null;
        }


        Timer = 0;
        yield return new WaitForSeconds(0.3f);

        Vector3 direction = ((Vector2)(CharacterControl.instance.transform.position - transform.position)).normalized;
        edgeColl.enabled = true;
        LR.enabled = true;
        GameFunction.t_Vector3 = transform.position;
        GameFunction.t_Vector3.z += 0.01f;
        LR.SetPosition(0, GameFunction.t_Vector3);
        edgeColl.points = edgePos;
        Vector2 _targetPos;
        while (Timer < duration)
        {
            Timer += Time.deltaTime;
            float t = Timer / duration;

            SR_heart.color = Color.Lerp(Color_laser, originColor_heart,GameFunction.smoothLerp(t));
            //发射射线
            hitPoint = Physics2D.Raycast(transform.position, direction, 999, mask);
            if(hitPoint.transform != null)
            {
                _targetPos = hitPoint.point;
            }
            else
            {
                _targetPos = transform.position + direction * 999;
            }

            float t_width = Mathf.Lerp(startWidth_laser, 0, t);
            LR.SetWidth(t_width, t_width);
            GameFunction.t_Vector3 = transform.position;
            GameFunction.t_Vector3.z += 0.01f;
            LR.SetPosition(0, GameFunction.t_Vector3);
            GameFunction.t_Vector3 = _targetPos;
            GameFunction.t_Vector3.z += 0.01f;
            edgePos[1] = transform.InverseTransformPoint(GameFunction.t_Vector3);
            edgeColl.points = edgePos;
            LR.SetPosition(1, GameFunction.t_Vector3);

            yield return null;
        }
        isLaser = false;
        LR.enabled = false;
        edgeColl.enabled = false;
        isAttackFinish = true;
    }

    /// <summary>
    /// 冲刺攻击
    /// </summary>
    /// <param name="_targetPos"></param>
    /// <returns></returns>
    IEnumerator IE_dash(Vector3 _targetPos)
    {
        float duration = ((Vector2)(_targetPos - transform.position)).magnitude / dashSpeed;
        float timer = 0;
        Vector3 t_Pos_start = transform.position;
        _targetPos.z = t_Pos_start.z;
        //准备
        const float time_ready = 0.5f;
        while(timer < time_ready)
        {
            timer += Time.deltaTime;
            float t = timer / time_ready;

            transform.localScale = Vector3.Lerp(originScale, originScale * 1.4f, GameFunction.smoothLerp(t));

            yield return null;
        }

        timer = 0;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            transform.localScale = Vector3.Lerp(originScale * 1.4f, originScale, GameFunction.smoothLerp(t));
            transform.position = Vector3.Lerp(t_Pos_start, _targetPos, t);

            yield return null;
        }

        isAttackFinish = true;
    }

    /// <summary>
    /// 眼睛跟随移动
    /// </summary>
    /// <param name="Pos_player"></param>
    void EyeFollowPlayer(Vector3 Pos_player)
    {
        Vector2 direction = ((Vector2)(Pos_player - transform.position)).normalized;
        const float factor_eye = 0.06f, factor_eyeCenter = 0.035f;
        const float speed = 0.1f;

        //获得目标点
        Vector3 _targetPos_eye = direction * factor_eye;
        Vector3 _targetPos_eyeCenter = direction * factor_eyeCenter;
        _targetPos_eye.z = -0.001f;
        _targetPos_eyeCenter.z = -0.001f;

        //移动
        Trans_eye.localPosition = Vector3.MoveTowards(Trans_eye.localPosition, _targetPos_eye, speed * Time.deltaTime);
        Trans_eyeCenter.localPosition = Vector3.MoveTowards(Trans_eyeCenter.localPosition, _targetPos_eyeCenter, speed * Time.deltaTime);
    }

    void PlayHurtParticle()
    {
        Particle_hurt.transform.position = transform.position + Vector3.forward * 0.01f;
        Particle_hurt.Play();
    }

    private void OnBecameVisible()
    {
        this.enabled = true;
    }

    private void OnBecameInvisible()
    {
        this.enabled = false;
    }
}
