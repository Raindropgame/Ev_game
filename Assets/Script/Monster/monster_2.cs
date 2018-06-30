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
    public monster_2_state currentState = monster_2_state.idle;
    [HideInInspector]
    public dir Dir = dir.left;
    public Transform eye;
    public Vector2 walkSpeed;
    public Rigidbody2D bullet;  //子弹
    public Transform ShootPos;
    public Vector2 bulletSpeed;
    public float idleTime;
    public GameObject deadParticle;

    private float _time0 = 0;
    private void FixedUpdate()
    {
        switch(currentState)
        {
            case monster_2_state.idle:

                _time0 += Time.deltaTime;

                if(isSeePlayer() && _time0 > idleTime / 2)
                {
                    if (bullet.gameObject.activeSelf == false)
                    {
                        _time0 = 0;
                        currentState = monster_2_state.attack;
                        animator.SetTrigger("attack");
                    }
                }

                if(_time0 > idleTime)
                {
                    _time0 = 0;
                    currentState = monster_2_state.walk;
                    animator.SetTrigger("walk");
                }
                break;
            case monster_2_state.attack:
                if(isFinishPlay())
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
                rig.velocity = walkSpeed * (Dir == dir.left ? -1 : 1);  //徘徊

                if(isSeePlayer())
                {
                    rig.velocity = Vector2.zero;
                    currentState = monster_2_state.idle;
                    animator.SetTrigger("idle");
                }
                break;
        }

        if (isNearEdge() || NearWall() < 0.2f)  //转换方向
        {
            Dir = (Dir == dir.left ? dir.right : dir.left);
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    void attack()  //攻击
    {

    }

    bool isSeePlayer()  //是否看到了主角
    {
        int mask = ~(1 << 10);
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
        RaycastHit2D HitPoint = Physics2D.Raycast(eye.position, Vector2.down, 2f, layerMask);
        if (HitPoint.distance <= 0)
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

    override public void _getHurt(int damage, Attribute attibute)
    {
        currentHP -= damage;
        if (currentHP <= 0)  //是否死亡
        {
            StartCoroutine(die());
            return;
        }
        StartCoroutine(beHurt());
    }

    IEnumerator die()  //死亡
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
