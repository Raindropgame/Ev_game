using UnityEngine;
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
    public Transform[] LeftHitTrans, RightHitTrans, HeadHitTrans;  //检测碰地的射线点
    public float DoubleAttackSpaceTime = 0.5f;
    public bool[] isEnable = new bool[12];  //管理人物某些功能   (依赖外部)
    public bool isHurt = false;  //人物是否处在受伤时期

    private Rigidbody2D rig;
    private float LeftKeyDown = 0, RightKeyDown = 0;  //检测是否双击键盘
    private int LeftOrRight = 0;  //2:右  1:左  ---检测双击所用
    private int _jumpTimes = 0; //记录跳跃次数
    private float JumpAccelerateTime = 0; //记录跳跃加速的时间  按键越久跳跃越高
    private float XJumpSpeed, YJumpSpeed, Yacceleration;
    private BoxCollider2D _collider;
    private CircleCollider2D _circleCollider;
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
    private int layerMask = 1 << 9;  //检测指定层
    private float _hurtTime = 0; //用于记录受伤时间
    private float jumpshoot_backTime = 0.05f,_time3 = 0;  //跳射后退时间

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        Init();
        rig = this.GetComponent<Rigidbody2D>();
        XJumpSpeed = Walkspeed + 1;
        YJumpSpeed = JumpSpeed;
        Yacceleration = JumpSpeed / MaxJumpTime;
        _collider = this.GetComponent<BoxCollider2D>();
        _circleCollider = this.GetComponent<CircleCollider2D>();
        lastDir = dir.right;
        SpriteRenderer = this.GetComponent<SpriteRenderer>();
        animator = this.GetComponent<Animator>();
        lastState = currentState;
    }

    private void Update()
    {
        isJump = !OnGround();  //测试
        rig.velocity = new Vector2(0, rig.velocity.y);
        switch (currentState)
        {
            case state.normal:
                changeDir();  // 行走
                if (isDoubleKeyDown() != 0 && isEnable[(int)state.run])
                {
                    currentState = state.run;  //双击变为奔跑
                }
                if(Input.GetKeyDown(KeyCode.Space) && isEnable[(int)state.jump])
                {
                    currentState = state.jump;    //跳跃
                    XJumpSpeed = Walkspeed + 1;
                }

                attack(); // 攻击
                shoot(); //射击
                Throw();  //扔
                dash(); //冲刺

                if(OnGround() == false)
                {
                    currentState = state.fall;  //落下
                    XJumpSpeed = Walkspeed + 1;
                    YJumpSpeed = 0;
                }
                break;
            case state.walk:
                currentState = state.normal;    //闲置
                changeDir();
                if (isDoubleKeyDown() != 0 && isEnable[(int)state.run])
                {
                    currentState = state.run;    //奔跑
                }
                if (Input.GetKeyDown(KeyCode.Space) && isEnable[(int)state.jump])
                {
                    currentState = state.jump;    //跳跃
                    XJumpSpeed = Walkspeed + 1;
                }

                dash(); //冲刺
                Throw();  //扔
                attack(); // 攻击
                shoot(); //射击

                if (OnGround() == false)
                {
                    currentState = state.fall;  //落下
                    XJumpSpeed = Walkspeed + 1;
                    YJumpSpeed = 0;
                }
                break;
            case state.run:

                if(! (CharacterAttribute.GetInstance().Breath >= CharacterAttribute.GetInstance().expend_run * Time.deltaTime))
                {
                    currentState = state.normal;
                    break;  //如果气息不够则退回站立状态
                }
                CharacterAttribute.GetInstance().Breath -= CharacterAttribute.GetInstance().expend_run * Time.deltaTime;  //奔跑气息消耗

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    rig.velocity += new Vector2(-RunSpeed, 0);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    rig.velocity += new Vector2(RunSpeed, 0);
                }

                if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
                {
                    currentState = state.normal;    //闲置
                }

                attack(); // 攻击
                shoot(); //射击
                Throw();  //扔
                dash(); //冲刺

                if (Input.GetKeyDown(KeyCode.Space) && isEnable[(int)state.jump])
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
                currentState = state.fall;    //下落
                if(Input.GetKey(KeyCode.Space) && JumpAccelerateTime < MaxJumpTime && CharacterAttribute.GetInstance().Breath >= CharacterAttribute.GetInstance().expend_jump * Time.deltaTime)
                {
                    CharacterAttribute.GetInstance().Breath -= CharacterAttribute.GetInstance().expend_jump * Time.deltaTime;  //气息消耗
                    JumpAccelerateTime += Time.deltaTime;
                    //YJumpSpeed = YJumpSpeed - Mathf.Pow(Yacceleration * Time.deltaTime,3);
                    YJumpSpeed = (-JumpSpeed / Mathf.Pow(MaxJumpTime,2)) * Mathf.Pow(JumpAccelerateTime,2 )+ JumpSpeed;
                    
                    if (YJumpSpeed < 0)
                    {
                        YJumpSpeed = 0;
                    }
                    rig.velocity = new Vector2(rig.velocity.x, YJumpSpeed);
                    currentState = state.jump;
                }
                JumpMove(); //跳跃时移动

                if(attack())  //攻击
                {
                    rig.velocity = new Vector2(rig.velocity.x, 0);
                    YJumpSpeed = 0;
                }
                shoot(); //射击
                if(Throw())  //扔
                {
                    rig.velocity = new Vector2(rig.velocity.x, 0);
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
                if(attack()) //攻击
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
                rig.velocity = new Vector2(Dir == dir.left ? -DashSpeed : DashSpeed, 0);
                if(_dashTime > DashTime)
                {
                    currentState = isJump ? state.fall : state.endDash;
                    YJumpSpeed = 0;
                    _dashTime = 0;
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
                    _time3 = 0;
                    currentState = state.fall;
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
                JumpMove();
                YJumpSpeed = YJumpSpeed + Yacceleration * Time.deltaTime;
                if(YJumpSpeed > JumpSpeed)
                {
                    YJumpSpeed = JumpSpeed;
                }
                rig.velocity = new Vector2(rig.velocity.x, -YJumpSpeed);

                if (Input.GetKeyDown(KeyCode.X) && isEnable[(int)state.attack1] && CharacterAttribute.GetInstance().Breath >= CharacterAttribute.GetInstance().expend_attack)  //攻击
                {
                    CharacterAttribute.GetInstance().Breath -= CharacterAttribute.GetInstance().expend_attack;
                    currentState = state.attack1;  //攻击
                    CharacterObjectManager.instance.attack1(Dir);
                }

                if(Input.GetKeyDown(KeyCode.Space) && _jumpTimes < JumpTimes && isEnable[(int)state.jump])   //连跳
                {
                    currentState = state.jump;
                    JumpAccelerateTime = 0;
                    YJumpSpeed = JumpSpeed;
                    rig.velocity = new Vector2(0, 0);
                    _jumpTimes++;
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
                if(!isPlayAnimation())
                {
                    currentState = state.normal;
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
            SpriteRenderer.flipX = SpriteRenderer.flipX == false ? true : false;
            changeCollider();
        }
        lastDir = Dir;

        changeAnimation(); //改变当前动画
        lastState = currentState;

    }

    void changeDir()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rig.velocity += new Vector2(-Walkspeed, 0);
            currentState = state.walk;
            Dir = dir.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
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
        if (Input.GetKeyDown(KeyCode.RightArrow))
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
        if (Input.GetKeyDown(KeyCode.LeftArrow))
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
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            rig.velocity = new Vector2(-XJumpSpeed, rig.velocity.y);
            Dir = dir.left;
        }
        if(Input.GetKey(KeyCode.RightArrow))
        {
            rig.velocity = new Vector2(XJumpSpeed, rig.velocity.y);
            Dir = dir.right;
        }
    }

    bool OnGround()  //判断是否在地上
    {
        int Tdir;
        if(Dir == dir.left)  //首先检测方向
        {
            Tdir = 0;
        }
        else
        {
            Tdir = 1;
        }

        RaycastHit2D LeftHit = Physics2D.Raycast(LeftHitTrans[Tdir].position, Vector2.down, 0.1f ,layerMask);
        RaycastHit2D RightHit = Physics2D.Raycast(RightHitTrans[Tdir].position, Vector2.down, 0.1f,layerMask);
        //Debug.DrawRay(LeftHitTrans[Tdir].position, Vector2.down,Color.red);
        //Debug.DrawRay(RightHitTrans[Tdir].position, Vector2.down,Color.red);
        int i = 0;
        if (LeftHit.transform != null)
        {
            if (LeftHit.transform.tag == "maps")
            {
                i++;
            }
        }
        if (RightHit.transform != null)
        {
            if (RightHit.transform.tag == "maps")
            {
                i++;
            }
        }
        if (i != 0)
        {
            YJumpSpeed = JumpSpeed;
            JumpAccelerateTime = 0;  // 落地 重新计算已加速的时间
            dashTimes = 0;  //空中冲刺次数重新计为0
            JumpShootTimes = 0;  //归零空中射击次数
            if (lastState == state.fall)
            {
                _jumpTimes = 0;  //归零跳跃次数
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    bool AtTop()  //判断头是否顶到墙
    {
        RaycastHit2D HeadHit = Physics2D.Raycast(HeadHitTrans[Dir == dir.left ? 0 : 1].position, Vector2.up, 0.2f, layerMask);
        if(HeadHit.transform != null)
        {
            if(HeadHit.transform.tag == "maps")
            {
                return true;
            }
        }
        return false;
    }

    void changeCollider()
    {
        float[] LeftOffSet = { -0.376f, 1.753f }, RightOffSet = { 0.509f, 1.753f };
        float[] LeftSize = { 1.241f, 3.82f }, RightSize = { 1.645f, 3.82f };
        float[] RightOffSet_2 = { 0.49f, 1.91f }, LeftOffSet_2 = { -0.37f, 1.82f };
        float RightRadius = 0.97f, LeftRadius = 0.81f;
        if(Dir == dir.left)
        {
            _collider.offset = new Vector2(LeftOffSet[0], LeftOffSet[1]);
            _collider.size = new Vector2(LeftSize[0], LeftSize[1]);
            _circleCollider.offset = new Vector2(LeftOffSet_2[0], LeftOffSet_2[1]);
            _circleCollider.radius = LeftRadius;
        }
        else
        {
            _collider.offset = new Vector2(RightOffSet[0], RightOffSet[1]);
            _collider.size = new Vector2(RightSize[0], RightSize[1]);
            _circleCollider.offset = new Vector2(RightOffSet_2[0], RightOffSet_2[1]);
            _circleCollider.radius = RightRadius;
        }
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
        if(Input.GetKeyDown(KeyCode.X) && isEnable[(int)state.attack1] && CharacterAttribute.GetInstance().Breath >= CharacterAttribute.GetInstance().expend_attack)
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
        if(Input.GetKeyDown(KeyCode.Z) && isEnable[(int)state.shoot])
        {
            if (isJump)
            {
                if (JumpShootTimes < MaxJumpShootTimes && CharacterAttribute.GetInstance().Breath >= CharacterAttribute.GetInstance().expend_jumpshoot)
                {
                    currentState = state.jumpshoot;
                    CharacterObjectManager.instance.arrow_2();
                    YJumpSpeed = 0;
                    rig.velocity = new Vector2(0, 0);  //停滞在空中
                    JumpShootTimes++;
                    CharacterAttribute.GetInstance().Breath -= CharacterAttribute.GetInstance().expend_jumpshoot;   //气息消耗
                }
            }
            else
            {
                if (CharacterAttribute.GetInstance().Breath >= CharacterAttribute.GetInstance().expend_shoot)
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
        if(Input.GetKeyDown(KeyCode.S) && CharacterAttribute.GetInstance().isEnable[(int)state.Throw] && CharacterAttribute.GetInstance().Breath > CharacterAttribute.GetInstance().expend_throw)
        {
            currentState = state.Throw;
            CharacterAttribute.GetInstance().Breath -= CharacterAttribute.GetInstance().expend_throw;
            return true;
        }
        return false;
    }

    bool dash()
    {
        if (Input.GetKeyDown(KeyCode.C) && isEnable[(int)state.dash] && CharacterAttribute.GetInstance().Breath >= CharacterAttribute.GetInstance().expend_dash && dashTimes < 1)
        {
            dashTimes++;
            currentState = state.dash;  //冲刺
            CharacterAttribute.GetInstance().Breath -= CharacterAttribute.GetInstance().expend_dash;
            CharacterObjectManager.instance.dash();
            return true;
        }
        return false;
    }

    void arrow()
    {
        CharacterObjectManager.instance.arrow();
    }

    public bool hurt(int Damage,Attribute attribute)  //受伤调用函数
    {
        float hurt_contined_time = 1;  //无敌持续时间
        if(!isHurt)
        {
            CharacterAttribute.GetInstance().reduce_HP(Damage);
            isHurt = true;
            currentState = state.hurt;
            Invoke("end_invincibility", hurt_contined_time);
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
        additionalVelocity = t;
    }

} 
