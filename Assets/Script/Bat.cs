using UnityEngine;
using System.Collections;

public class Bat : Monster_base {

    private enum Bat_state
    {
        stay,
        dash,
        fly
    }

    public float dashSpeed;
    public float viewDistance = 10;
    public float flySpeed;
    public float flyCycle;
    public float waitTime_fly;
    public float MaxFlyTime;
    public GameObject deadParticle;

    private Bat_state currentState = Bat_state.stay;
    private RaycastHit2D hitPoint;
    private bool _isSeePlayer = false;
    private float random;
    private float Timer_fly = 0;
    private Vector2 dashDir;
    private float Timer_dash = 0;
    private float Time_stay = 4;
    private float Timer_stay = 0;
    private TrailRenderer TR;

    public override void onStart()
    {
        base.onStart();

        changeState(currentState);
        random = Random.value;
        TR = GetComponent<TrailRenderer>();
    }

    protected override void _FixedUpdate()
    {
        base._FixedUpdate();

        _isSeePlayer = isSeePlayer();

        switch(currentState)
        {
            case Bat_state.stay:

                if(_isSeePlayer)
                {
                    changeState(Bat_state.fly);
                    changeDir(getPlayerDir());
                }

                break;

            case Bat_state.fly:
                Timer_stay += Time.deltaTime;

                rig.velocity = GameFunction.getVector3(Mathf.Sin(Time.time / flyCycle), Mathf.Sin( 0.6f * Time.time / flyCycle + random), 0) * flySpeed;
                float distanceFromCiling = DistanceFromCeiling();

                if(distanceFromCiling > 1)
                {
                    rig.velocity += Vector2.up * flySpeed * 2;
                }

                if(_isSeePlayer)
                {
                    Timer_fly += Time.deltaTime;
                }
                else
                {
                    if(distanceFromCiling < 1 && Timer_stay > Time_stay)
                    {
                        changeState(Bat_state.stay);
                    }
                }

                if(Timer_fly > waitTime_fly)
                {
                    dashDir = (CharacterControl.instance.getCollCenter() - transform.position).normalized;
                    changeState(Bat_state.dash);
                    transform.Rotate(GameFunction.getVector3(0, 0, getAngle(dashDir)));
                }

                changeDir(getPlayerDir());
                break;

            case Bat_state.dash:
                Timer_dash += Time.deltaTime;

                rig.velocity = dashDir * dashSpeed;

                if(isNearWall(dashDir) == true || Timer_dash > MaxFlyTime)
                {
                    changeState(Bat_state.fly);
                    rig.velocity = Vector2.zero;
                    Timer_fly = 0;
                    transform.rotation = Quaternion.Euler(Vector3.zero);
                    Timer_dash = 0;
                    TR.enabled = false;
                }
                break;
        }
    }

    void changeState(Bat_state t)
    {
        currentState = t;
        animator.SetTrigger(t.ToString());

        if(t == Bat_state.fly)
        {
            Timer_stay = 0;
        }
        else
        {
            if(t == Bat_state.dash)
            {
                TR.enabled = true;
            }
        }

        rig.velocity = Vector2.zero;
    }

    void changeDir(dir t)
    {
        if(Dir != t)
        {
            Dir = t;
            transform.localScale = GameFunction.getVector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    Vector2 dir_view = Vector3.zero;
    bool isSeePlayer()
    {
        if((CharacterControl.instance.transform.position - transform.position).sqrMagnitude < Mathf.Pow(viewDistance,2))
        {
            int mask = (1 << 9) | (1 << 17);
            dir_view = CharacterControl.instance.transform.position - transform.position;
            hitPoint = Physics2D.Raycast(transform.position, dir_view, viewDistance, mask);

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

    dir getPlayerDir()
    {
        if(CharacterControl.instance.transform.position.x > transform.position.x)
        {
            return dir.right;
        }
        else
        {
            return dir.left;
        }
    }

    float getAngle(Vector2 t)
    {
        float originX = t.x;
        if(t.x > 0)
        {
            t.x = -t.x;
        }
        return Vector2.Angle(Vector2.up, t) * (originX > 0 ? -1 : 1);
    }

    bool isNearWall(Vector2 rayDir)
    {
        int mask = 1 << 9;
        hitPoint = Physics2D.Raycast(transform.position, rayDir, 2f, mask);
        if(hitPoint.transform != null)
        {
            return true;
        }
        return false;
    }

    float DistanceFromCeiling()
    {
        int mask = 1 << 9;
        hitPoint = Physics2D.Raycast(transform.position, Vector2.up, 1000.0f, mask);
        if(hitPoint.transform != null)
        {
            return hitPoint.distance;
        }
        return 1000;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag.CompareTo("Player") == 0 && isHurtPlayer)
        {
            CharacterControl.instance.hurt(1,Attribute.normal);
        }
    }

    override public void _getHurt(int damage, Attribute attribute, Vector2 ColliderPos)
    {
        currentHP -= damage;

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
}
