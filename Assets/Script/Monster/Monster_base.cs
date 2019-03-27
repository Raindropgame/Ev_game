using UnityEngine;
using System.Collections;

public class Monster_base : MonoBehaviour {

    //怪物的基类
    //死亡、行为、受伤由派生类处理

    [HideInInspector]
    public Color hurtColor;
    public int MaxHP;
    [HideInInspector]
    public int currentHP;
    public Collider2D[] colliderID;   //一个怪物提供多个碰撞体
    [Header("是否打开击退效果")]
    public bool isUseHitBack = false;
    public float backVelocity;
    public float backTime;
    [Header("是否检测在雨中的状态")]
    public bool isGetRainState = true;
    [Header("---------------")]


    protected Texture texture;
    protected SpriteRenderer SR;
    protected Animator animator;
    protected Rigidbody2D rig;
    protected ArrayList abnormalState = new ArrayList();
    protected bool isEnable = true;  //是否运行fixedupdate
    protected Vector2 additionalVelocity = Vector2.zero;
    [HideInInspector]
    public dir Dir = dir.left;
    protected bool isHurtPlayer = true; //是否对玩家产生伤害
    private bool m_isInRain = false;  //是否在雨中
    protected bool isInRain
    {
        get
        {
            getIsInRain();
            return m_isInRain;

        }
        set
        {
            m_isInRain = value;
        }
    }
    protected bool isInShelter = false;

    //资源----
    static private GameObject m_effect_lightning = null;
    static protected GameObject effect_lightning
    {
        get
        {
            if(m_effect_lightning == null)
            {
                m_effect_lightning = Resources.Load<GameObject>("Effect_Lightning");
                Resources.UnloadUnusedAssets();
            }
            return m_effect_lightning;
        }
    }

    static private GameObject m_burningEffect = null;
    static protected GameObject burningEffect
    {
        get
        {
            if(m_burningEffect == null)
            {
                m_burningEffect = Resources.Load<GameObject>("fire_burning");
                Resources.UnloadUnusedAssets();
            }
            return m_burningEffect;
        }
    }

    static private Material m_stone_material = null;
    static protected Material stone_material
    {
        get
        {
            if(m_stone_material == null)
            {
                m_stone_material = Resources.Load<Material>("Petrochemical");
                Resources.UnloadUnusedAssets();
            }
            return m_stone_material;
        }
    }

    static private GameObject m_stone_piece = null;
    static protected GameObject stone_piece
    {
        get
        {
            if(m_stone_piece == null)
            {
                m_stone_piece = Resources.Load<GameObject>("stone_piece");
                Resources.UnloadUnusedAssets();
            }
            return m_stone_piece;
        }
    }

    static private GameObject m_iceFrag = null;
    static protected GameObject iceFrag
    {
        get
        {
            if(m_iceFrag == null)
            {
                m_iceFrag = Resources.Load<GameObject>("iceFrag");
                Resources.UnloadUnusedAssets();
            }
            return m_iceFrag;
        }
    }

    static private Sprite[] m_iceCube = new Sprite[2] { null, null };
    static protected Sprite[] iceCube
    {
        get
        {
            if(m_iceCube[0] == null)
            {
                m_iceCube[0] = Resources.LoadAll<Sprite>("iceCube")[0];
            }
            if(m_iceCube[1] == null)
            {
                m_iceCube[1] = Resources.LoadAll<Sprite>("iceCube")[1];
            }
            return m_iceCube;
        }
    }

    static private GameObject m_state_lightning = null;
    static protected GameObject state_lightning
    {
        get
        {
            if(m_state_lightning == null)
            {
                m_state_lightning = Resources.Load<GameObject>("state_lightning");
                Resources.UnloadUnusedAssets();
            }
            return m_state_lightning;
        }
    }
    //--------

    private void Start()  //初始化
    {
        rig = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        currentHP = MaxHP;
        CharacterObjectManager._sendHurt += getHurt;  //受伤消息(来自玩家)注册
        animator = GetComponent<Animator>();

        onStart();
        
        texture = SR == null ? null : SR.sprite.texture; //获得纹理
    }

    private void FixedUpdate()
    {

        if (isEnable)
        {
            //是否在雨中


            _FixedUpdate();

            CalAdditionalVelocity();  //计算额外速度
        }
    }

    virtual protected void _FixedUpdate()
    {    }

    private void OnDestroy()
    {
        CharacterObjectManager._sendHurt -= getHurt;  //解除绑定
    }

    public void getHurt(int damage, Attribute attribute,int gameobjectID, Vector2 ColliderPos)   //接口
    {
        bool isTrigger = false;
        for (int i = 0; i < colliderID.Length; i++)  //有多个碰撞体
        {
            if (gameobjectID == colliderID[i].gameObject.GetInstanceID())
            {
                isTrigger = true;
            }
        }

        if (isTrigger)  //是否是被攻击的对象
        {
            switch (attribute)   //根据受伤属性改变被攻击的颜色
            {
                case Attribute.normal:
                    hurtColor = Color.white;
                    break;
                case Attribute.fire:
                    hurtColor = GameData.fireColor;
                    break;
                case Attribute.ice:
                    hurtColor = GameData.iceColor;
                    break;
                case Attribute.wood:
                    hurtColor = GameData.woodColor;
                    break;
                case Attribute.lightning:
                    hurtColor = GameData.lightningColor;
                    break;
                default:
                    hurtColor = Color.white;
                    break;
            }
            _getHurt(damage, attribute,ColliderPos);
        }
    }

    virtual public IEnumerator beHurt()  //受伤反馈
    {
        //被攻击时的闪烁
        MaterialPropertyBlock matBlock = new MaterialPropertyBlock();
        matBlock.SetColor("_hitColor", hurtColor);
        matBlock.SetFloat("_isHit", 1);
        matBlock.SetTexture("_MainTex", texture);
        SR.SetPropertyBlock(matBlock);
        yield return new WaitForSeconds(0.1f);  //变色的时间
        matBlock.SetFloat("_isHit", 0);
        SR.SetPropertyBlock(matBlock);
        //----------
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.CompareTo("lighting") == 0)   //被雷电击中
        {
            _getHurt(GameData.lightningDamage, Attribute.lightning,Vector2.zero);
        }
        else
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("shelter"))
            {
                isInShelter = true;
            }
        }

        TriggerEnter(collision);
    }

    virtual public void TriggerEnter(Collider2D collision)   
    {

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("shelter"))
        {
            isInShelter = false;
        }

        TriggerExit(collision);
    }

    virtual public void TriggerExit(Collider2D collision)
    {

    }

    virtual public void onStart()  //派生类中调用的start虚函数
    {

    }

    virtual public void _getHurt(int damage, Attribute attribute, Vector2 ColliderPos)  //受伤虚函数
    {
        if (currentHP <= 0)  //是否死亡
        {
            //StopAllCoroutines();  //死亡后停止所有协程
            StartCoroutine(die());
            return;
        }

        if(isUseHitBack)  //被击退效果
        {
            if(ColliderPos == Vector2.zero)  //判断方向
            {

            }
            else
            {
                Vector2 direction = Vector2.zero;
                for (int i = 0;i<colliderID.Length;i++)
                {
                    direction = ((Vector2)colliderID[i].bounds.center - ColliderPos).normalized;  //攻击物指向被攻击物
                }
                StartCoroutine(hitBack(backTime,direction));
            }
        }
    }

    virtual protected IEnumerator die()
    {
        yield return null;
    }

    protected bool isFinishPlay()  //是否播放完当前动画
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
        {
            return true;
        }
        return false;
    }

    protected RaycastHit2D hitPoint;
    private Collider2D m_gravity;
    protected Collider2D gravity
    {
        get
        {
            if (m_gravity == null)
            {
                m_gravity = GameFunction.GetGameObjectInChildrenByName(this.gameObject, "Gravity").GetComponent<Collider2D>();
            }
            return m_gravity;
        }
        set
        {
            m_gravity = value;
        }
    }
    protected bool isGround()  //是否落地
    {
        

        LayerMask layerMask = 1 << 9;
        hitPoint = Physics2D.Raycast((Vector2)gravity.bounds.min, Vector2.down, 0.05f, layerMask);
        if (hitPoint.transform != null)
        {
            return true;
        }
        hitPoint = Physics2D.Raycast((Vector2)gravity.bounds.min + Vector2.right * gravity.bounds.size.y, Vector2.down, 0.05f, layerMask);
        if (hitPoint.transform != null)
        {
            return true;
        }
        return false;
    }

    virtual protected void OnFrozenStart()
    { }
    virtual protected void OnFrozenEnd()
    { }
    protected IEnumerator frozen()  //冰冻
    {
        if(abnormalState.Contains(AbnormalState.frozen) || abnormalState.Contains(AbnormalState.stone))  //是否已经被冰冻
        {
            yield break;
        }

        OnFrozenStart();

        abnormalState.Add(AbnormalState.frozen);
        isHurtPlayer = false;

        MaterialPropertyBlock MB = new MaterialPropertyBlock();
        SR.GetPropertyBlock(MB);
        MB.SetFloat("_isFroze", 1);
        SR.SetPropertyBlock(MB);

        animator.enabled = false;
        isEnable = false;
        rig.velocity = Vector2.zero;
        float originGravity = rig.gravityScale;  //添加重力
        rig.gravityScale = GameData.gravityScale_stone;

        float _time1 = 0;
        int _currentHP = currentHP;
        while(_time1<GameData.frozenTime)  //被攻击提前碎掉
        {
            _time1 += Time.deltaTime;
            SR.SetPropertyBlock(MB);

            if (_currentHP != currentHP)
            {
                MB.SetFloat("_isFroze", 0);
                SR.SetPropertyBlock(MB);
                rig.gravityScale = originGravity; //还原重力
                animator.enabled = true;
                isEnable = true;
                if (abnormalState.Contains(AbnormalState.stone))  //是否被石化
                {
                    rig.gravityScale = GameData.gravityScale_stone;
                    animator.enabled = false;
                }
                abnormalState.Remove(AbnormalState.frozen);
                GameObject _t_iceFrag = Instantiate(iceFrag, position: transform.position, rotation: Quaternion.Euler(0, 0, 0)) as GameObject;
                _t_iceFrag.transform.parent = transform;
                _t_iceFrag.transform.localScale = Vector3.one;
                yield return new WaitForSeconds(_t_iceFrag.GetComponent<ParticleSystem>().duration);
                Destroy(_t_iceFrag);
                if(!abnormalState.Contains(AbnormalState.stone))
                {
                    isHurtPlayer = true;
                }
                else
                {
                    isHurtPlayer = false;
                }

                OnFrozenEnd();
                yield break;
            }

            if (currentHP <= 0)  //已死亡
                break;

            yield return null;
        }

        OnFrozenEnd();
        isHurtPlayer = true;
        MB.SetFloat("_isFroze", 0);
        SR.SetPropertyBlock(MB);

        rig.gravityScale = originGravity; //还原重力
        animator.enabled = true;
        isEnable = true;
        if (abnormalState.Contains(AbnormalState.stone))  //是否被石化
        {
            rig.gravityScale = GameData.gravityScale_stone;
            animator.enabled = false;
        }
        abnormalState.Remove(AbnormalState.frozen);
        GameObject t_iceFrag = Instantiate(iceFrag, position: transform.position, rotation: Quaternion.Euler(0, 0, 0)) as GameObject;
        t_iceFrag.transform.parent = transform;
        t_iceFrag.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(t_iceFrag.GetComponent<ParticleSystem>().duration);
        Destroy(t_iceFrag);
    }

    virtual protected void OnPetrochemicalStart()
    { }
    virtual protected void OnPetrochemicalEnd()
    { }
    protected IEnumerator petrochemical()  //石化
    {
        if(abnormalState.Contains(AbnormalState.stone))
        {
            yield break;
        }
        abnormalState.Add(AbnormalState.stone);

        OnPetrochemicalStart();

        float originGravity = rig.gravityScale;
        float originDrag = rig.drag;

        Material originMaterial = SR.material;

        rig.velocity = Vector2.zero;
        SR.material = stone_material;
        this.enabled = false;
        animator.enabled = false;
        isHurtPlayer = false;
        for(int i = 0;i<colliderID.Length;i++)
        {
            colliderID[i].isTrigger = false;
        }
        this.gameObject.tag = "maps";
        this.gameObject.layer = LayerMask.NameToLayer("terrain");
        //重力与摩擦力
        rig.gravityScale = GameData.gravityScale_stone;
        rig.drag = GameData.dragScale_stone;
        //-----

        //石化持续
        yield return new WaitForSeconds(GameData.stoneTime);

        OnPetrochemicalEnd();

        SR.material = originMaterial;
        this.enabled = true;
        animator.enabled = true;
        if (abnormalState.Contains(AbnormalState.frozen))
        {
            this.enabled = false;
            animator.enabled = false;
        }
        isHurtPlayer = true;
        for (int i = 0; i < colliderID.Length; i++)
        {
            colliderID[i].isTrigger = true;
        }
        this.gameObject.tag = "enemy";
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        //重力与摩擦力 
        rig.gravityScale = originGravity;
        rig.drag = originDrag;
        //-----
        Instantiate(stone_piece, position: SR.bounds.center + new Vector3(0,0,-1), rotation: Quaternion.Euler(0, 0, 0));

        abnormalState.Remove(AbnormalState.stone);
    }

    protected IEnumerator burning()   //灼烧
    {
        if(abnormalState.Contains(AbnormalState.burning))
        {
            yield break;
        }

        abnormalState.Add(AbnormalState.burning);
        float burnTime = GameData.burningTime;
        float burnSpaceTime = GameData.burningSpaceTime;

        float _time1 = 0, _time2 = 0;
        while(_time1 < burnTime)  //灼烧循环
        {
            _time1 += Time.deltaTime;
            _time2 += Time.deltaTime;

            if(abnormalState.Contains(AbnormalState.stone) || abnormalState.Contains(AbnormalState.frozen))
            {
                break;
            }
            else
            {
                if(_time2 > burnSpaceTime)
                {
                    Instantiate(burningEffect, position: SR.bounds.center + new Vector3(0, 0, -1), rotation: Quaternion.Euler(0, 0, 0));
                    getHurt(GameData.burningDamage, Attribute.fire, this.gameObject.GetInstanceID(), Dir == dir.left?(Vector2)colliderID[0].bounds.center - Vector2.one: (Vector2)colliderID[0].bounds.center + Vector2.one);
                    _time2 = 0;

                    if(currentHP <= 0)
                    {
                        break;
                    }
                }
            }
            yield return null;
        }

        abnormalState.Remove(AbnormalState.burning); //移去异常状态
    }

    protected IEnumerator electricShock()  //感电
    {
        if(abnormalState.Contains(AbnormalState.electric))
        {
            yield break;
        }
        abnormalState.Add(AbnormalState.electric);

        GameObject t_state_lightning = Instantiate(state_lightning, position: SR.bounds.center + new Vector3(0, SR.bounds.size.y * 0.7f, 0), rotation: Quaternion.Euler(0, 0, 0)) as GameObject;
        t_state_lightning.transform.SetParent(transform, true);
        GameObject t_effect_lightning = Instantiate(effect_lightning, position: SR.bounds.center, rotation: Quaternion.Euler(0, 0, Random.Range(0,360))) as GameObject;
        t_effect_lightning.SetActive(false);

        //获取数据
        float lightningTime = GameData.lightningTime;
        float _time1 = 0,_time2 = 0;
        float lightning_Space_Time = GameData.lightningSpace;
        float odds = GameData.lightning_Odds;

        while(_time1 < lightningTime)
        {
            _time1 += Time.deltaTime;
            _time2 += Time.deltaTime;

            if(currentHP <= 0)
            {
                break;
            }

            if(_time2 > lightning_Space_Time)
            {
                Random.InitState(Time.frameCount);
                if(Random.value < odds)
                {
                    t_effect_lightning.SetActive(true);
                    t_effect_lightning.transform.position = SR.bounds.center + new Vector3(0,0,-0.1f);
                }

                _time2 = 0;
            }

            yield return null;
        }

        Destroy(t_effect_lightning);
        Destroy(t_state_lightning);

        abnormalState.Remove(AbnormalState.electric);
    }

    public void addVelocity(Vector2 velocity)
    {
        additionalVelocity += velocity;
    }

    void CalAdditionalVelocity()
    {
        if (rig != null)
        {
            rig.velocity += additionalVelocity;
            additionalVelocity = Vector2.zero;
        }
    }

    IEnumerator hitBack(float time, Vector2 direction)
    {
        float _time = 0;
        while (_time < time)
        {
            _time += Time.deltaTime;
            float t = 1 - (_time / time);

            rig.velocity = Vector2.zero;
            //if (direction.x < 0)
            //{
            //    additionalVelocity += -backVelocity.x * t * direction;
            //}
            //else
            //{
            //    additionalVelocity += backVelocity.x * t * direction;
            //}
            additionalVelocity += backVelocity * t * direction;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        try
        {
            Gizmos.DrawWireCube(SR.bounds.center, SR.bounds.size);
        }
        catch
        {

        }
    }

    //改变方向
    protected void changeDir(dir d)
    {
        if (Dir != d)
        {
            Dir = d;
            GameFunction.t_Vector3.Set(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            transform.localScale = GameFunction.t_Vector3;
        }
    }

    void getIsInRain()
    {
        if (isGetRainState)
        {
            if (!isInShelter)
            {
                if (WeatherData.getIntance().currentWeather == weather.Rain || WeatherData.getIntance().currentWeather == weather.RainAndThunder)
                {
                    isInRain = true;
                }
            }
        }
    }
}
