using UnityEngine;
using System.Collections;

public class monster_2 : Monster_base {

	//魔花
    //看见玩家就攻击
    //玩家碰到会受伤
    //玩家靠近会后退
    //未看见玩家 徘徊

    public enum monster_2_state
    {
        walk,
        idle,
        attack
    }

    [HideInInspector]
    public monster_2_state currentState = monster_2_state.walk;
    public Transform eye;
    public Vector2 walkSpeed;
    public Rigidbody2D bullet;  //子弹
    public Transform ShootPos;
    public Vector2 bulletSpeed;
    public float idleTime;
    public GameObject deadParticle;

    private float _time0 = 0;
    private Vector2 _walkSpeed;

    protected override void _FixedUpdate()
    {
        base._FixedUpdate();

        switch (currentState)
        {
            case monster_2_state.idle:

                _time0 += Time.deltaTime;

                if (isSeePlayer() && _time0 > idleTime / 2)
                {
                    if (bullet.gameObject.activeSelf == false)
                    {
                        _time0 = 0;
                        currentState = monster_2_state.attack;
                        animator.SetTrigger("attack");
                    }
                }

                if (_time0 > idleTime)
                {
                    _time0 = 0;
                    currentState = monster_2_state.walk;
                    animator.SetTrigger("walk");
                }
                break;
            case monster_2_state.attack:
                if (isFinishPlay())
                {
                    currentState = monster_2_state.idle;
                    animator.SetTrigger("idle");

                    bullet.gameObject.SetActive(true);

                    bullet.gameObject.transform.position = ShootPos.position;
                    bullet.gameObject.transform.parent = null;
                    bullet.velocity = new Vector2(bulletSpeed.x * (Dir == dir.left ? -1 : 1), bulletSpeed.y);
                }
                break;
            case monster_2_state.walk:
                _walkSpeed.x = walkSpeed.x * (Dir == dir.left ? -1 : 1);
                _walkSpeed.y = rig.velocity.y;
                rig.velocity = _walkSpeed;  //徘徊

                if (isSeePlayer())
                {
                    rig.velocity = Vector2.zero;
                    currentState = monster_2_state.idle;
                    animator.SetTrigger("idle");
                }
                break;
        }

        if (isGround())
        {
            if (isNearEdge() || NearWall() < 0.2f)  //转换方向
            {
                Dir = (Dir == dir.left ? dir.right : dir.left);
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        }
    }


    bool isSeePlayer()  //是否看到了主角
    {
        int mask = (1 << 0) | (1 << 9);  //检测特定层
        RaycastHit2D HitPoint = Physics2D.Raycast(eye.position, Dir == dir.right ? Vector2.right : Vector2.left, 15f, mask);
        if (HitPoint.transform != null)
        {
            if (HitPoint.transform.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    bool isNearEdge()   //是否在边上
    {
        LayerMask layerMask = 1 << 9;
        RaycastHit2D HitPoint = Physics2D.Raycast(eye.position, Vector2.down, 1f, layerMask);
        if (HitPoint.transform == null)
        {
            return true;
        }
        return false;
    }

    float NearWall()  //检测离墙的距离
    {
        LayerMask layerMask = 1 << 9;
        RaycastHit2D HitPoint = Physics2D.Raycast(eye.position, Dir == dir.left ? Vector2.left : Vector2.right, 1f, layerMask);
        if (HitPoint.transform == null)
        {
            return 1000f;
        }
        return HitPoint.distance;
    }

    override public void _getHurt(int damage, Attribute attribute, Vector2 ColliderPos)
    {
        currentHP -= damage;

        if (attribute == Attribute.fire)
        {
            if (abnormalState.Contains(AbnormalState.frozen))
            {
                CameraFollow.instance.Stop(0.17f,0.1f);  //屏幕特效
                StartCoroutine(CameraFollow.instance.shakeCamera(0.25f, 0.04f, 0.2f));  //镜头抖动
                GameObject t = Resources.Load<GameObject>("fire");
                Instantiate(t, position: SR.bounds.center, rotation: Quaternion.Euler(0, 0, 0));
                currentHP -= damage;  //双倍伤害
            }
        }

        base._getHurt(damage, attribute,ColliderPos);
        if (currentHP <= 0)
        {
            return;
        }

        if (attribute == Attribute.ice)  //冰冻
        {
            StartCoroutine(frozen());
        }
        if(attribute == Attribute.wood)
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

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.tag == "Player")
        {
            CharacterControl.instance.hurt(1, Attribute.normal);
        }
    }

}
