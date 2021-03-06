﻿using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{

    public static CharacterControl instance;

    public float Walkspeed;   //  (依赖外部)
    public float RunSpeed;    //(依赖外部)
    public float JumpSpeed;  //跳跃 与  最大下落速度
    public float DashSpeed;
    public float DashTime;
    public float MaxJumpTime;
    public float HurtTime;  //受伤僵直时间
    public int JumpTimes = 1;    //(依赖外部)
    public int MaxJumpShootTimes = 1;  //(依赖外部)
    public state currentState = state.normal;
    public float DoubleKeySapceTime = 0.5f;
    public dir Dir = dir.right;
    public float DoubleAttackSpaceTime = 0.5f;
    public bool[] isEnable = new bool[12];  //管理人物某些功能   (依赖外部)
    public bool isHurt = false;  //人物是否处在受伤时期
    public float DashInertiaTime = 0.1f;
    public Vector3 jumpScale;
    public float jumpScale_Time = 0.2f;
    public float proportion_dashScale = 0.4f;
    public Vector3 dashScale;
    public float jumpTimeFactor = 1f;
    public bool isPlatJump = false;  //连跳平台

    [HideInInspector]
    public Rigidbody2D rig;
    private float LeftKeyDown = 0, RightKeyDown = 0;  //检测是否双击键盘
    private int LeftOrRight = 0;  //2:右  1:左  ---检测双击所用
    [HideInInspector]
    public int _jumpTimes = 0; //记录跳跃次数
    private float JumpAccelerateTime = 0; //记录跳跃加速的时间  按键越久跳跃越高
    private float XJumpSpeed, YJumpSpeed, Yacceleration;
    [HideInInspector]
    public BoxCollider2D _collider;
    private dir lastDir;  //上一帧的方向
    private SpriteRenderer SpriteRenderer;
    private Animator animator;
    private state lastState;  //上一个动作  用于状态机
    private bool isJump = false;
    private float _DoubleAttackSpaceTime = 0;
    private bool isAttack = false;
    private float _dashTime = 0;
    private int dashTimes = 0;  //记录空中冲刺次数
    private int JumpShootTimes = 0;  //记录控制射击次数
    private float _hurtTime = 0; //用于记录受伤时间
    private float jumpshoot_backTime = 0.05f,_time3 = 0;  //跳射后退时间
    private bool isJumpShootBack = false; //是否开始跳射后退
    private bool isInShelter = false;  //是否在避雨的区域中
    [HideInInspector]
    public bool isInRain = false;
    private bool isGetInput = true;
    private float _Timer_dash = 0;
    private float _Timer_JumpScale;
    private Vector3 originScale;
    private float Timer_dashScale = 0;
    private RaycastHit2D hitPoint;
    private float Time_pressJumpKey = 0;
    private bool canJump = false;

    public delegate void HurtHandler();
    public static event HurtHandler Event_hurt;

    private void Awake()
    {
        instance = this;

        _collider = this.GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        Init();
        rig = this.GetComponent<Rigidbody2D>();
        XJumpSpeed = Walkspeed + 2.5f;
        YJumpSpeed = JumpSpeed;
        Yacceleration = JumpSpeed / MaxJumpTime;
        lastDir = dir.right;
        SpriteRenderer = this.GetComponent<SpriteRenderer>();
        animator = this.GetComponent<Animator>();
        lastState = currentState;
        originScale = transform.localScale;
    }

    private void Update()
    {
        isJump = !OnGround();  //测试
        isInRain = checkIsInRain(); //是否在雨中

        if (isHorizontalBounce)
        {
            rig.velocity = new Vector2(rig.velocity.x, rig.velocity.y);
        }
        else
        {
            rig.velocity = new Vector2(0, rig.velocity.y);
        }

        switch (currentState)
        {
            case state.normal:
                changeDir();  // 行走
                if ((isDoubleKeyDown() != 0 || MyInput.instance.isGetRun())&& isEnable[(int)state.run] && !CharacterAttribute.GetInstance().isOverLoad_breath)
                {
                    currentState = state.run;  //双击变为奔跑
                }
                if(MyInput.instance.isGetJumpDown() && isEnable[(int)state.jump] && isGetInput)
                {
                    currentState = state.jump;    //跳跃
                    XJumpSpeed = Walkspeed + 2.5f;
                }

                attack(); // 攻击
                shoot(); //射击
                Throw();  //扔
                dash(); //冲刺

                if(OnGround() == false)
                {
                    currentState = state.fall;  //落下
                    XJumpSpeed = Walkspeed + 2.5f;
                    YJumpSpeed = 0;
                }
                break;
            case state.walk:
                currentState = state.normal;    //闲置
                changeDir();
                if ((isDoubleKeyDown() != 0 || MyInput.instance.isGetRun())&& isEnable[(int)state.run] && !CharacterAttribute.GetInstance().isOverLoad_breath)
                {
                    currentState = state.run;    //奔跑
                }
                if (MyInput.instance.isGetJumpDown() && isEnable[(int)state.jump] && isGetInput)
                {
                    currentState = state.jump;    //跳跃
                    XJumpSpeed = Walkspeed + 2.5f;
                }

                dash(); //冲刺
                Throw();  //扔
                attack(); // 攻击
                shoot(); //射击

                if (OnGround() == false)
                {
                    currentState = state.fall;  //落下
                    XJumpSpeed = Walkspeed + 2.5f;
                    YJumpSpeed = 0;
                }
                break;
            case state.run:

                if((CharacterAttribute.GetInstance().isOverLoad_breath))
                {
                    currentState = state.normal;
                    break;  //如果气息不够则退回站立状态
                }
                CharacterAttribute.GetInstance().Breath -= CharacterAttribute.GetInstance().expend_run * Time.deltaTime;  //奔跑气息消耗

                if (MyInput.instance.isGetLeft() && isGetInput)
                {
                    rig.velocity += new Vector2(-RunSpeed, 0);
                    Dir = dir.left;
                }
                if (MyInput.instance.isGetRight() && isGetInput)
                {
                    rig.velocity += new Vector2(RunSpeed, 0);
                    Dir = dir.right;
                }

                if (MyInput.instance.isGetLeftUp() || MyInput.instance.isGetRightUp() && isGetInput)
                {
                    currentState = state.normal;    //闲置
                }

                attack(); // 攻击
                shoot(); //射击
                Throw();  //扔
                dash(); //冲刺

                if (MyInput.instance.isGetJumpDown() && isEnable[(int)state.jump] && isGetInput)
                {
                    currentState = state.jump;    //跳跃
                    XJumpSpeed = RunSpeed + 1;
                }

                if (OnGround() == false)
                {
                    currentState = state.fall;   //落下
                    XJumpSpeed = RunSpeed + 1;
                    YJumpSpeed = 0;
                }
                break;
            case state.jump:
                if(JumpAccelerateTime < 0.01f)
                {
                    _Timer_JumpScale = 0;
                }

                isDoubleKeyDown();  //计时
                currentState = state.fall;    //下落

                if (isPlatJump)  //连跳
                {
                    isPlatJump = false;
                    JumpAccelerateTime = 0;
                    YJumpSpeed = JumpSpeed;
                    rig.velocity = Vector2.zero;
                    _jumpTimes++;
                    canJump = false;
                    Time_pressJumpKey = 0;
                }

                //是否在跳跃（按键）
                if (MyInput.instance.isGetJump() && isGetInput)
                {
                    Time_pressJumpKey += Time.deltaTime;
                    canJump = true;
                }
                else
                {
                    canJump = false;
                }
                if (Time_pressJumpKey > MaxJumpTime * (XJumpSpeed < RunSpeed ? 0.92f : 1.0f) * 0.6f)   //剩余50%时间自动跳跃
                {
                    canJump = true;
                }

                if (JumpAccelerateTime < MaxJumpTime * (XJumpSpeed < RunSpeed ? 0.92f : 1.0f) && !CharacterAttribute.GetInstance().isOverLoad_breath && canJump)
                {
                    CharacterAttribute.GetInstance().Breath -= CharacterAttribute.GetInstance().expend_jump * Time.deltaTime;  //气息消耗
                    JumpAccelerateTime += (Time.deltaTime * jumpTimeFactor);
                    //YJumpSpeed = YJumpSpeed - Mathf.Pow(Yacceleration * Time.deltaTime,3);
                    YJumpSpeed = ((-JumpSpeed / Mathf.Pow(MaxJumpTime * (XJumpSpeed < RunSpeed ? 0.92f : 1.0f), 2)) * Mathf.Pow(JumpAccelerateTime, 2) + JumpSpeed);
                    
                    if (YJumpSpeed < 0)
                    {
                        YJumpSpeed = 0;
                    }
                    rig.velocity = GameFunction.getVector2(rig.velocity.x, YJumpSpeed);
                    currentState = state.jump;
                }
                JumpMove(); //跳跃时移动

                //跳跃形变
                _Timer_JumpScale += (Time.deltaTime * jumpTimeFactor);
                float t_jumpScale = _Timer_JumpScale / (jumpScale_Time);
                transform.localScale = Vector3.Lerp(Dir == dir.right?jumpScale:GameFunction.getVector3(-jumpScale.x,jumpScale.y,jumpScale.z), Dir == dir.right?originScale:GameFunction.getVector3(-originScale.x,originScale.y,originScale.z), t_jumpScale);

                if(attack())  //攻击
                {
                    rig.velocity = GameFunction.getVector2(rig.velocity.x, 0);
                    YJumpSpeed = 0;
                }
                shoot(); //射击
                if(Throw())  //扔
                {
                    rig.velocity = GameFunction.getVector2(rig.velocity.x, 0);
                    YJumpSpeed = 0;
                }

                if (dash())  //冲刺
                {
                    dashTimes = 1;
                }

                if (AtTop())  //判断是否顶到墙
                {
                    currentState = state.fall;
                    YJumpSpeed = 0;
                }

                if(currentState == state.fall)  //使动作更加自然
                {
                    YJumpSpeed = 0;
                }

                break;
            case state.dash:

                isDoubleKeyDown();  //计时
                if (attack()) //攻击
                {
                    YJumpSpeed = 0;
                    _dashTime = 0;
                }
                if(shoot()) //射击
                {
                    YJumpSpeed = 0;
                    _dashTime = 0;
                }

                _dashTime += Time.deltaTime;
                Timer_dashScale += Time.deltaTime;

                float t_dashScale = Timer_dashScale / (proportion_dashScale * DashTime);
                transform.localScale = Vector3.Lerp(Dir == dir.right ? dashScale : GameFunction.getVector3(-dashScale.x, dashScale.y, dashScale.z), Dir == dir.right ? originScale : GameFunction.getVector3(-originScale.x, originScale.y, originScale.z), t_dashScale);
                rig.velocity = new Vector2(Dir == dir.left ? -DashSpeed : DashSpeed, 0);
                if(_dashTime > DashTime)
                {
                    currentState = isJump ? state.fall : state.endDash;
                    YJumpSpeed = 0;
                    _dashTime = 0;

                    //完全冲刺后产生惯性
                    if(isJump)
                    {
                        StartCoroutine(IE_dashInertia());
                    }
                    else
                    {
                        _Timer_dash = 0;
                    }
                }
                break;
            case state.attack1:
                //  空中攻击移动-----------------
                if (isJump)
                {
                    //JumpMove();
                    YJumpSpeed = YJumpSpeed + Yacceleration * Time.deltaTime;
                    if (YJumpSpeed > JumpSpeed)
                    {
                        YJumpSpeed = JumpSpeed;
                    }
                    rig.velocity = new Vector2(rig.velocity.x, -YJumpSpeed);
                }
                //---------------------------------

                if (!isPlayAnimation())
                {
                    if (isJump)
                    {
                        currentState = state.fall;
                    }
                    else
                    {
                        currentState = state.normal;
                    }
                }
                break;
            case state.attack2:
                //  空中攻击移动-----------------
                if (isJump)
                {
                    //JumpMove();
                    YJumpSpeed = YJumpSpeed + Yacceleration * Time.deltaTime;
                    if (YJumpSpeed > JumpSpeed)
                    {
                        YJumpSpeed = JumpSpeed;
                    }
                    rig.velocity = new Vector2(rig.velocity.x, -YJumpSpeed);
                }
                //---------------------------------

                if (!isPlayAnimation())
                {
                    if (isJump)
                    {
                        currentState = state.fall;
                    }
                    else
                    {
                        currentState = state.normal;
                    }
                }
                break;
            case state.jumpshoot:
                if (isJumpShootBack)
                {
                    _time3 += Time.deltaTime;  //后退效果
                    if (_time3 < jumpshoot_backTime)
                    {
                        rig.velocity = new Vector2(Dir == dir.left ? 20 : -20, 18);
                    }
                    else
                    {
                        rig.velocity = Vector2.zero;
                    }

                    if (!isPlayAnimation())
                    {
                        isJumpShootBack = false;
                        _time3 = 0;
                        currentState = state.fall;
                    }
                }
                break;
            case state.shoot:              
                if(!isPlayAnimation())
                {
                    currentState = state.normal;
                }
                break;
            case state.hurt:
                int t = 20;

                _hurtTime += Time.deltaTime;
                rig.velocity = new Vector2((Dir == dir.left ? (-t / HurtTime * _hurtTime + t) : -(-t / HurtTime * _hurtTime + t)), 0);

                YJumpSpeed = YJumpSpeed + Yacceleration * Time.deltaTime;   //为Y轴添加重力
                if (YJumpSpeed > JumpSpeed)
                {
                    YJumpSpeed = JumpSpeed;
                }
                rig.velocity = new Vector2(rig.velocity.x, -YJumpSpeed);

                if (_hurtTime > HurtTime)
                {
                    currentState = isJump == true ? state.fall : state.normal;
                    _hurtTime = 0;
                }
                break;
            case state.fall:
                isDoubleKeyDown();  //计时
                if(!isHorizontalBounce)
                {
                    JumpMove();  //水平弹飞中不能移动
                }
                YJumpSpeed = YJumpSpeed + Yacceleration * Time.deltaTime * jumpTimeFactor;
                if(YJumpSpeed > JumpSpeed)
                {
                    YJumpSpeed = JumpSpeed;
                }

                if (!isBounce)
                {
                    rig.velocity = GameFunction.getVector2(rig.velocity.x, -YJumpSpeed);
                }
                
                if(isBounce)
                {
                    AtTop();
                }

                if (MyInput.instance.isGetAttack() && isEnable[(int)state.attack1] && !CharacterAttribute.GetInstance().isOverLoad_breath && isGetInput)  //攻击
                {
                    CharacterAttribute.GetInstance().Breath -= CharacterAttribute.GetInstance().expend_attack;
                    currentState = state.attack1;  //攻击
                    CharacterObjectManager.instance.attack1(Dir);
                }

                if((MyInput.instance.isGetJumpDown() && _jumpTimes < JumpTimes && isEnable[(int)state.jump] && isGetInput) || isPlatJump)  //连跳
                {
                    isPlatJump = false;
                    currentState = state.jump;
                    JumpAccelerateTime = 0;
                    YJumpSpeed = JumpSpeed;
                    rig.velocity = Vector2.zero;
                    _jumpTimes++;
                    canJump = false;
                    Time_pressJumpKey = 0;
                }

                shoot(); //射击
                Throw();  //扔
                 
                if (dash())  //冲刺
                {
                    dashTimes = 1;
                }

                if (OnGround()) // 判断是否落地
                {
                    currentState = state.normal;
                }
                break;
            case state.endDash:
                //惯性
                _Timer_dash += Time.deltaTime;
                float t_endDash = _Timer_dash / DashInertiaTime;
                rig.velocity = Vector2.Lerp((Dir == dir.left ? Vector2.left : Vector2.right) * DashSpeed, Vector2.zero, t_endDash);

                if(_Timer_dash > DashInertiaTime)
                {
                    currentState = state.normal;
                    //播放粒子特效
                    if (!isJump)
                    {
                        CharacterObjectManager.instance.PlayFallParticle(isInRain);
                    }
                }
                break;
            case state.Throw:
                //  空中移动-----------------
                if (isJump)
                {
                    YJumpSpeed = YJumpSpeed + Yacceleration * Time.deltaTime;
                    if (YJumpSpeed > JumpSpeed)
                    {
                        YJumpSpeed = JumpSpeed;
                    }
                    rig.velocity = new Vector2(rig.velocity.x, -YJumpSpeed);
                }
                //---------------------------------

                if (!isPlayAnimation())
                {
                    currentState = isJump? state.fall:state.normal;
                }
                break;
        }

        if(isAttack)  // 为双击计时 
        {
            _DoubleAttackSpaceTime += Time.deltaTime;
            if(_DoubleAttackSpaceTime > DoubleAttackSpaceTime)
            {
                isAttack = false;
                _DoubleAttackSpaceTime = 0;
            }
        }

        if (isHurt)  //受伤
        {
            flash();  // 人物闪烁
        }

        rig.velocity += additionalVelocity;  //计算额外的速度
        additionalVelocity = Vector2.zero;

        if(lastDir != Dir) // 翻转图片  判断减少运算量
        {
            //SpriteRenderer.flipX = SpriteRenderer.flipX == false ? true : false;
            GameFunction.t_Vector3 = transform.localScale;
            GameFunction.t_Vector3.x *= -1;
            transform.localScale = GameFunction.t_Vector3;
        }
        lastDir = Dir;

        changeAnimation(); //改变当前动画
        CharacterObjectManager.instance.changeDustParticle(currentState, lastState, isJump, isInRain, Dir);  //改变尘土粒子效果
        correctJumpScale();  //修正跳跃形变
        correntDashScale();  //修正冲刺形变
        lastState = currentState;

    }

    void changeDir()
    {
        if (MyInput.instance.isGetLeft() && isGetInput)
        {
            rig.velocity += new Vector2(-Walkspeed, 0);
            currentState = state.walk;
            Dir = dir.left;
        }
        if (MyInput.instance.isGetRight() && isGetInput)
        {
            rig.velocity += new Vector2(Walkspeed, 0);
            currentState = state.walk;
            Dir = dir.right;
        }
    }

    int isDoubleKeyDown()
    {

        switch (LeftOrRight)
        {
            case 1:
                LeftKeyDown += Time.deltaTime;
                if (LeftKeyDown >= DoubleKeySapceTime)
                {
                    LeftOrRight = 0;
                    LeftKeyDown = 0;
                }
                break;
            case 2:
                RightKeyDown += Time.deltaTime;
                if (RightKeyDown >= DoubleKeySapceTime)
                {
                    LeftOrRight = 0;
                    RightKeyDown = 0;
                }
                break;
        }
        if (MyInput.instance.isGetRightDown() && isGetInput)
        {
            if (LeftOrRight == 2 && RightKeyDown < DoubleKeySapceTime)
            {
                LeftOrRight = 0;
                RightKeyDown = 0;
                return 2;
            }
            else
            {
                LeftOrRight = 2;
            }
        }
        if (MyInput.instance.isGetLeftDown() && isGetInput)
        {
            if (LeftOrRight == 1 && LeftKeyDown < DoubleKeySapceTime)
            {
                LeftOrRight = 0;
                LeftKeyDown = 0;
                return 1;
            }
            else
            {
                LeftOrRight = 1;
            }
        }
        return 0;
    }

    void JumpMove()
    {
        if(MyInput.instance.isGetLeft() && isGetInput)
        {
            rig.velocity = GameFunction.getVector2(-XJumpSpeed, rig.velocity.y);
            Dir = dir.left;
        }
        if(MyInput.instance.isGetRight() && isGetInput)
        {
            rig.velocity = GameFunction.getVector2(XJumpSpeed, rig.velocity.y);
            Dir = dir.right;
        }
    }

    bool OnGround()  //判断是否在地上
    {
        int mask = 1 << LayerMask.NameToLayer("terrain");
        hitPoint = Physics2D.Raycast(_collider.bounds.min, Vector2.down, 0.2f, mask);
        if(hitPoint.transform != null)
        {
            YJumpSpeed = JumpSpeed;
            if (currentState == state.fall)  //避免角落跳跃BUG
            {
                JumpAccelerateTime = 0;  // 落地 重新计算已加速的时间
            }
            dashTimes = 0;  //空中冲刺次数重新计为0
            JumpShootTimes = 0;  //归零空中射击次数
            if (lastState == state.fall)
            {
                _jumpTimes = 0;  //归零跳跃次数
                stopBounce();
            }
            canJump = false;
            Time_pressJumpKey = 0;
            return true;
        }
        hitPoint = Physics2D.Raycast((Vector2)_collider.bounds.min + _collider.bounds.size.x * Vector2.right, Vector2.down, 0.2f, mask);
        if(hitPoint.transform != null)
        {
            YJumpSpeed = JumpSpeed;
            if (currentState == state.fall)
            {
                JumpAccelerateTime = 0;  // 落地 重新计算已加速的时间
            }
            dashTimes = 0;  //空中冲刺次数重新计为0
            JumpShootTimes = 0;  //归零空中射击次数
            if (lastState == state.fall)
            {
                _jumpTimes = 0;  //归零跳跃次数
                stopBounce();
            }
            canJump = false;
            Time_pressJumpKey = 0;
            return true;
        }
        return false;
    }

    bool AtTop()  //判断头是否顶到墙
    {
        int mask = 1 << LayerMask.NameToLayer("terrain");
        hitPoint = Physics2D.Raycast((Vector2)_collider.bounds.max + Vector2.left * _collider.bounds.size.x, Vector2.up, 0.1f, mask);
        if(hitPoint.transform != null)
        {
            stopBounce();
            return true;
        }
        hitPoint = Physics2D.Raycast(_collider.bounds.max, Vector2.up, 0.1f, mask);
        if (hitPoint.transform != null)
        {
            stopBounce();
            return true;
        }
        return false;
    }


    bool isPlayAnimation()  //当前动画是否播放完毕？
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
        {
            return false;
        }
        return true;
    }

    void changeAnimation()
    {
        if (lastState != currentState)
        {
            animator.SetTrigger(currentState.ToString());           
        }
    }

    bool attack()  //封装攻击
    {
        if(MyInput.instance.isGetAttack() && isEnable[(int)state.attack1] && !CharacterAttribute.GetInstance().isOverLoad_breath && isGetInput)
        {
            if(isAttack && _DoubleAttackSpaceTime < DoubleAttackSpaceTime)
            {
                currentState = state.attack2;
                isAttack = false;
                _DoubleAttackSpaceTime = 0;
                CharacterObjectManager.instance.attack2(Dir);
            }
            else
            {
                currentState = state.attack1;
                isAttack = true;
                _DoubleAttackSpaceTime = 0;
                CharacterObjectManager.instance.attack1(Dir);
            }
            CharacterAttribute.GetInstance().Breath -= CharacterAttribute.GetInstance().expend_attack; //气息消耗
            return true;
        }
        return false;
    }

    bool shoot()   //射击
    {
        if(MyInput.instance.isGetShoot() && isEnable[(int)state.shoot] && isGetInput)
        {
            if (isJump)
            {
                if (JumpShootTimes < MaxJumpShootTimes && !CharacterAttribute.GetInstance().isOverLoad_breath)
                {
                    currentState = state.jumpshoot;
                    //CharacterObjectManager.instance.arrow_2();
                    YJumpSpeed = 0;
                    rig.velocity = new Vector2(0, 0);  //停滞在空中
                    JumpShootTimes++;
                    CharacterAttribute.GetInstance().Breath -= CharacterAttribute.GetInstance().expend_jumpshoot;   //气息消耗
                }
            }
            else
            {
                if (!CharacterAttribute.GetInstance().isOverLoad_breath)
                {
                    currentState = state.shoot;
                    CharacterAttribute.GetInstance().Breath -= CharacterAttribute.GetInstance().expend_shoot;   //气息消耗
                    Invoke("arrow", 0.2f);
                }
            }
            return true;
        }
        return false;
    }

    bool Throw()   //扔
    {
        if(MyInput.instance.isGetThrow() && CharacterAttribute.GetInstance().isEnable[(int)state.Throw] && !CharacterAttribute.GetInstance().isOverLoad_breath && isGetInput)
        {
            currentState = state.Throw;
            CharacterAttribute.GetInstance().Breath -= CharacterAttribute.GetInstance().expend_throw;
            return true;
        }
        return false;
    }

    bool dash()
    {
        if (MyInput.instance.isGetDash() && isEnable[(int)state.dash] && !CharacterAttribute.GetInstance().isOverLoad_breath && dashTimes < 1 && isGetInput)
        {
            dashTimes++;
            currentState = state.dash;  //冲刺
            CharacterAttribute.GetInstance().Breath -= CharacterAttribute.GetInstance().expend_dash;
            CharacterObjectManager.instance.dash();
            Timer_dashScale = 0;
            return true;
        }
        return false;
    }

    void arrow()
    {
        CharacterObjectManager.instance.arrow();
    }

    public bool hurt(int Damage,Attribute attribute,Vector3 CollCenter)  //受伤调用函数
    {
        float hurt_contined_time = 1;  //无敌持续时间
        if(!isHurt)
        {
            //方向
            if(CollCenter.x > transform.position.x)
            {
                Dir = dir.right;
            }
            else
            {
                Dir = dir.left;
            }

            if (Event_hurt != null)
            {
                Event_hurt();
            }
            CharacterAttribute.GetInstance().reduce_HP(Damage);
            isHurt = true;
            currentState = state.hurt;
            _dashTime = 0;
            Invoke("end_invincibility", hurt_contined_time);
            CharacterObjectManager.instance.BeHurt();  //开启图片动画
            return true;
        }
        return false;
    }
    public bool hurt(int Damage, Attribute attribute)  //受伤调用函数(不指定方向)
    {
        float hurt_contined_time = 1;  //无敌持续时间
        if (!isHurt)
        {
            if (Event_hurt != null)
            {
                Event_hurt();
            }
            CharacterAttribute.GetInstance().reduce_HP(Damage);
            isHurt = true;
            currentState = state.hurt;
            _dashTime = 0;
            Invoke("end_invincibility", hurt_contined_time);
            CharacterObjectManager.instance.BeHurt();  //开启图片动画
            return true;
        }
        return false;
    }

    void end_invincibility()  //结束受伤状态
    {
        SpriteRenderer.color = new Color(1, 1, 1, 1);
        isHurt = false;
    }

    private float _time2 = 0;  //辅助 闪烁 计时
    public void flash() //人物闪烁
    {
        float spaceTime = 0.1f;  //闪烁间隔
        _time2 += Time.deltaTime;
        if (_time2 > spaceTime)
        {
            SpriteRenderer.color = SpriteRenderer.color.a < 0.6f ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);  //该处修改闪烁透明度
            _time2 = 0;
        }
    }

    void Init()  //重新读取人物属性
    {
        JumpTimes = CharacterAttribute.GetInstance().JumpTimes;
        MaxJumpShootTimes = CharacterAttribute.GetInstance().MaxJumpShootTimes;
        isEnable = CharacterAttribute.GetInstance().isEnable;
    }

    private Vector2 additionalVelocity = Vector2.zero;
    public void add_Velocity(Vector2 t)
    {
        additionalVelocity += t;
    }

    //被弹开
    private bool isBounce = false,isHorizontalBounce = false;
    private Coroutine bounce_c = null;
    public void bounce(float time,float speed,dir dir)
    {
        if(!isBounce)
        {
            bounce_c = StartCoroutine(_Bounce(time, speed,dir));
        }
        else
        {
            stopBounce();
            bounce_c = StartCoroutine(_Bounce(time, speed, dir));
        }
    }

    IEnumerator _Bounce(float time,float speed,dir _dir)
    {
        float _time_bounce = 0;
        Vector2 v = Vector2.zero;
        switch (_dir)
        {
            case dir.down:
                v = new Vector2(0, -1);
                break;
            case dir.left:
                v = new Vector2(-1, 0);
                isHorizontalBounce = true;
                break;
            case dir.right:
                v = new Vector2(1, 0);
                isHorizontalBounce = true;
                break;
            case dir.top:
                v = new Vector2(0, 1);
                break;
        }
        isBounce = true;


        currentState = state.fall;
        _dashTime = 0;   //防止弹球BUG
        add_Velocity(v * speed);
        rig.velocity = Vector2.zero;
        Vector2 _v = Vector2.zero;
        _jumpTimes = -1;
        while (_time_bounce < time)
        {

            if(rig.velocity.x > 0 && _dir == dir.left)
            {
                break;
            }
            if (rig.velocity.x < 0 && _dir == dir.right)
            {
                break;
            }

            _time_bounce += Time.deltaTime;

            _v = speed / time * Time.deltaTime * v; 

            add_Velocity(-_v);


            yield return null;
        }
        _jumpTimes = 0;
        _time_bounce = 0;
        isBounce = false;
        YJumpSpeed = 0;
        isHorizontalBounce = false;
    }

    void stopBounce()  //停止弹跳协程
    {
        if(isBounce)
        {
            StopCoroutine(bounce_c);
            isBounce = false;
            isHorizontalBounce = false;
            _jumpTimes = 0;
        }
    }

    public void startJumpShootBack()
    {
        CharacterObjectManager.instance.arrow_2();
        isJumpShootBack = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("shelter"))
        {
            isInShelter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("shelter"))
        {
            isInShelter = false;
        }
    }

    bool checkIsInRain()  //检查是否在雨中
    {       
        if (!isInShelter)
        {
            if (WeatherData.getIntance().currentWeather == weather.Rain || WeatherData.getIntance().currentWeather == weather.RainAndThunder)
            {
                return true;
            }
        }
        return false;
    }

    public void setInputNone()
    {
        isGetInput = false;
    }

    public void getInput()
    {
        isGetInput = true;
    }

    //冲刺后的惯性
    IEnumerator IE_dashInertia()
    {
        float _timer0 = 0;
        float originSpeed = Mathf.Abs(rig.velocity.x);
        setInputNone();
        while(_timer0 < DashInertiaTime && currentState == state.fall)
        {
            _timer0 += Time.deltaTime;
            float t = _timer0 / DashInertiaTime;
            add_Velocity(Vector2.Lerp((Dir == dir.left ? Vector2.left : Vector2.right) * originSpeed, Vector2.zero, t));

            yield return null;
        }

        getInput();
    }

    //修正跳跃产生的形变
    void correctJumpScale()
    {
        if(currentState != state.jump && lastState == state.jump)
        {
            transform.localScale = Dir == dir.right ? originScale : GameFunction.getVector3(-originScale.x, originScale.y, originScale.z);
        }
    }

    //修正冲刺产生的形变
    void correntDashScale()
    {
        if(lastState == state.dash)
        {
            if(currentState != state.dash)
            {
                transform.localScale = Dir == dir.right?originScale:GameFunction.getVector3(-originScale.x,originScale.y,originScale.z);
            }
        }
    }

    public Vector3 getCollCenter()
    {
        return _collider.bounds.center;
    }

    //控制Door_1的钥匙
    private bool m_isGetDoor1_Key = false;
    public bool isGetDoor1_Key
    {
        set
        {
            m_isGetDoor1_Key = value;
        }
        get
        {
            return m_isGetDoor1_Key;
        }
    }
} 
