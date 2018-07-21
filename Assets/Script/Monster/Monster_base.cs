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
    [Header("是否为陆地行走生物")]
    public bool isLand = false;
    public Transform foot;

    protected Texture texture;
    protected SpriteRenderer SR;
    protected Animator animator;
    protected Rigidbody2D rig;
    protected ArrayList abnormalState = new ArrayList();
    protected bool isEnable = true;  //是否运行fixedupdate

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
            _FixedUpdate();
        }
    }

    virtual protected void _FixedUpdate()
    {    }

    private void OnDestroy()
    {
        CharacterObjectManager._sendHurt -= getHurt;  //解除绑定
    }

    public void getHurt(int damage, Attribute attribute,int gameobjectID)
    {
        bool isTrigger = false;
        for (int i = 0; i < colliderID.Length; i++)
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
                    hurtColor = GameData.getInstance().fireColor;
                    break;
                case Attribute.ice:
                    hurtColor = GameData.getInstance().iceColor;
                    break;
                case Attribute.wood:
                    hurtColor = GameData.getInstance().woodColor;
                    break;
                case Attribute.lightning:
                    hurtColor = GameData.getInstance().lightningColor;
                    break;
                default:
                    hurtColor = Color.white;
                    break;
            }
            _getHurt(damage, attribute);
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
        if(collision.tag == "lighting")   //被雷电击中
        {
            _getHurt(GameData.getInstance().lightningDamage, Attribute.lightning);
        }

        TriggerEnter(collision);
    }

    virtual public void TriggerEnter(Collider2D collision)   
    {

    }

    virtual public void onStart()  //派生类中调用的start虚函数
    {

    }

    virtual public void _getHurt(int damage, Attribute attribute)  //受伤虚函数
    {

    }

    protected bool isFinishPlay()  //是否播放完当前动画
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
        {
            return true;
        }
        return false;
    }

    protected bool isGround()  //是否落地
    {
        LayerMask layerMask = 1 << 9;
        RaycastHit2D HitPoint = Physics2D.Raycast(foot.position, Vector2.down, 2f, layerMask);
        if (HitPoint.distance <= 0.15f && (HitPoint.transform != null))
        {
            return true;
        }
        return false;
    }

    protected IEnumerator frozen()  //冰冻
    {
        float frozenTime = 1;
        if(abnormalState.Contains(AbnormalState.frozen))  //是否已经被冰冻
        {
            yield break;
        }

        abnormalState.Add(AbnormalState.frozen);  
        GameObject iceFrag = Resources.Load<GameObject>("iceFrag");  //加载粒子效果  
        //---添加图层
        GameObject t = new GameObject();
        SpriteRenderer t_SR = t.AddComponent<SpriteRenderer>();
        if (SR.bounds.extents.x > SR.bounds.extents.y)  //选择图片
        {
            t_SR.sprite = Resources.LoadAll<Sprite>("iceCube")[1];
        }
        else
        {
            t_SR.sprite = Resources.LoadAll<Sprite>("iceCube")[0];
        }

        Bounds t_bounds = SR.bounds,bounds = t_SR.bounds;
        t.transform.position = t_bounds.center + new Vector3(0,0,-0.01f);
        Vector2 scale = Vector2.zero;
        if(t_bounds.extents.x > t_bounds.extents.y)  //宽大于高
        {
            float _scale = t_bounds.extents.x / (bounds.extents.x * 0.41f);
            scale.x = _scale;
            scale.y = _scale;
        }
        else   //高大于宽
        {
            float _scale = t_bounds.extents.x / (bounds.extents.x * 0.41f);
            scale.x = _scale;
            scale.y = _scale;
        }
        t.transform.localScale = scale;
        t.transform.parent = transform;
        //----
        animator.enabled = false;
        isEnable = false;
        rig.velocity = Vector2.zero;

        float _time1 = 0;
        int _currentHP = currentHP;
        while(_time1<frozenTime)  //被攻击提前碎掉
        {
            _time1 += Time.deltaTime;

            if(_currentHP != currentHP)
            {
                animator.enabled = true;
                isEnable = true;
                if (abnormalState.Contains(AbnormalState.stone))  //是否被石化
                {
                    animator.enabled = false;
                }
                abnormalState.Remove(AbnormalState.frozen);
                GameObject _t_iceFrag = Instantiate(iceFrag, position: t.transform.position, rotation: Quaternion.Euler(0, 0, 0)) as GameObject;
                _t_iceFrag.transform.parent = transform;
                _t_iceFrag.transform.localScale = Vector3.one;
                Destroy(t);
                yield return new WaitForSeconds(_t_iceFrag.GetComponent<ParticleSystem>().duration);
                Destroy(_t_iceFrag);
                yield break;
            }
            yield return null;
        }

        animator.enabled = true;
        isEnable = true;
        if (abnormalState.Contains(AbnormalState.stone))  //是否被石化
        {
            animator.enabled = false;
        }
        abnormalState.Remove(AbnormalState.frozen);
        GameObject t_iceFrag = Instantiate(iceFrag, position: t.transform.position, rotation: Quaternion.Euler(0, 0, 0)) as GameObject;
        t_iceFrag.transform.parent = transform;
        t_iceFrag.transform.localScale = Vector3.one;
        Destroy(t);
        yield return new WaitForSeconds(t_iceFrag.GetComponent<ParticleSystem>().duration);
        Destroy(t_iceFrag);
    }

    protected IEnumerator petrochemical()  //石化
    {
        if(abnormalState.Contains(AbnormalState.stone))
        {
            yield break;
        }
        abnormalState.Add(AbnormalState.stone);

        float originGravity = rig.gravityScale;
        float originDrag = rig.drag;

        Material stone_material = Resources.Load<Material>("Petrochemical");
        Material originMaterial = SR.material;

        rig.velocity = Vector2.zero;
        SR.material = stone_material;
        this.enabled = false;
        animator.enabled = false;
        for(int i = 0;i<colliderID.Length;i++)
        {
            colliderID[i].isTrigger = false;
        }
        this.gameObject.tag = "maps";
        this.gameObject.layer = LayerMask.NameToLayer("terrain");
        //重力与摩擦力
        rig.gravityScale = GameData.getInstance().gravityScale_stone;
        rig.drag = GameData.getInstance().dragScale_stone;
        //-----

        yield return new WaitForSeconds(5);

        SR.material = originMaterial;
        this.enabled = true;
        animator.enabled = true;
        if (abnormalState.Contains(AbnormalState.frozen))
        {
            this.enabled = false;
            animator.enabled = false;
        }
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

        abnormalState.Remove(AbnormalState.stone);
    }
}
