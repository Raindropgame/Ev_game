using UnityEngine;
using System.Collections;

public class monster_4_Ground : Monster_base {

    //中立
    //头顶有一盏灯
    //看见其他东西会后退

    public enum monster_4_Ground_state
    {
        walk,
        idle,
        back,
    }

    public Transform eye;
    public Transform head;
    public float headDistance;
    public float walkSpeed;
    public float eyeDistance;
    public GameObject deadParticle;
    public float speedUpTime = 1f;

    private monster_4_Ground_state currentState = monster_4_Ground_state.idle;
    private bool _isSeePlayer = false;
    private bool _isNearEdge = false;
    private bool _isNearWall = false;
    private float _time0 = 0;
    private float motionDuration = 1.5f;
    private float _Timer_speed = 0;

    protected override void _FixedUpdate()
    {
        base._FixedUpdate();


        if (currentState != monster_4_Ground_state.back)
        {
            _isNearEdge = isNearEdge();
            _isNearWall = isNearWall();
            _isSeePlayer = isSeePlayer();
        }
        else
        {
            transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,transform.localScale.z);
            _isNearEdge = isNearEdge();
            _isNearWall = isNearWall();
            _isSeePlayer = isSeePlayer();
            
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        switch(currentState)
        {
            case monster_4_Ground_state.idle:

                if(_isSeePlayer)
                {
                    if (!_isNearEdge && !_isNearWall)
                    {
                        currentState = monster_4_Ground_state.walk;
                        animator.SetTrigger("walk");
                        _time0 = 0;
                        _Timer_speed = 0;
                    }
                    else
                    {
                        currentState = monster_4_Ground_state.back;
                        animator.SetTrigger("back");
                        _time0 = 0;
                        _Timer_speed = 0;
                    }
                }
                break;


            case monster_4_Ground_state.back:
                //惯性
                _Timer_speed += Time.deltaTime;
                float t = _Timer_speed / speedUpTime;
                rig.velocity = Vector2.down * Mathf.Lerp(0, walkSpeed, t);

                _time0 += Time.deltaTime;

                if (_isNearEdge || _isNearWall)
                {
                    currentState = monster_4_Ground_state.idle;
                    animator.SetTrigger("idle");
                    rig.velocity = Vector2.zero;
                }
                if(_time0 > motionDuration)
                {
                    currentState = monster_4_Ground_state.idle;
                    animator.SetTrigger("idle");
                    rig.velocity = Vector2.zero;
                }
                break;



            case monster_4_Ground_state.walk:
                _Timer_speed += Time.deltaTime;
                float t1 = _Timer_speed / speedUpTime;

                rig.velocity = Vector2.up * Mathf.Lerp(0, walkSpeed, t1);
                _time0 += Time.deltaTime;

                if(_isNearEdge || _isNearWall)
                {
                    currentState = monster_4_Ground_state.idle;
                    animator.SetTrigger("idle");
                    rig.velocity = Vector2.zero;
                }
                if (_time0 > motionDuration)
                {
                    currentState = monster_4_Ground_state.idle;
                    animator.SetTrigger("idle");
                    rig.velocity = Vector2.zero;
                }
                break;
        }
    }

    bool isSeePlayer()
    {
        int mask = (1 << 17) | (1 << 9);  //检测特定层
        Vector2 rayDir = eye.position.x > colliderID[0].bounds.center.x ? Vector2.right : Vector2.left;
        RaycastHit2D hitPoint = Physics2D.Raycast(eye.position, rayDir, eyeDistance, mask);
        if(hitPoint.transform != null)
        {
            if(hitPoint.transform.tag.CompareTo("Player") == 0)
            {
                return true;
            }
        }
        return false;
    }

    bool isNearEdge()
    {
        LayerMask layerMask = 1 << 9;
        Vector2 rayDir = eye.position.x < colliderID[0].bounds.center.x ? Vector2.right : Vector2.left;
        RaycastHit2D hitPoint = Physics2D.Raycast(head.position, rayDir, headDistance, layerMask);
        if(hitPoint.transform == null)
        {
            return true;
        }
        return false;
    }

    bool isNearWall()
    {
        LayerMask layerMask = 1 << 9;
        Vector2 rayDir = eye.position.x > colliderID[0].bounds.center.x ? Vector2.right : Vector2.left;
        RaycastHit2D hitPoint = Physics2D.Raycast(head.position, rayDir, colliderID[0].bounds.size.y, layerMask);
        if(hitPoint.transform != null)
        {
            return true;
        }
        return false;
    }

    override protected IEnumerator die()  //死亡
    {
        GameFunction.GetGameObjectInChildrenByName(this.gameObject, "Light").SetActive(false);
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
                CameraFollow.instance.Stop(GameData.fire_boom_stopTime, 0.1f);  //屏幕特效
                Screen1_render.instance.Wave(this.transform.position, 0.5f);
                CameraFollow.instance.shakeCamera(0.2f, 0.03f, 0.3f);  //镜头抖动
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
            if (!abnormalState.Contains(AbnormalState.stone))
            {
                StartCoroutine(electricShock());
            }
        }
        StartCoroutine(beHurt());
    }
}
