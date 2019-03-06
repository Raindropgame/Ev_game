using UnityEngine;
using System.Collections;

public class Pop : MonoBehaviour {

    public float speed, time;
    public dir _dir;
    public bool isOther = false;

    private float originSpeed;
    private Animator animator;
    private Collider2D coll;
    private SpriteRenderer SR;
    private const float add_Sccale = 1.15f;
    private const float coolingSpeed = 0.25f;
    private const float backSpeed = 1200.0f;

    private void Start()
    {
        originSpeed = speed;
        animator = GetComponent<Animator>();
        if(animator == null)
        {
            animator = transform.parent.GetComponentInChildren<Animator>();
        }
        coll = GetComponent<Collider2D>();
        SR = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isOther)
        {
            bool isColl = true;
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (_dir == dir.top)
                {
                    if (collision.contacts[i].point.y < coll.bounds.center.y)  //是否是来自碰撞器顶部
                    {
                        isColl = false;
                        break;
                    }
                }
                if (_dir == dir.left)
                {
                    if (collision.contacts[i].point.x > coll.bounds.center.x)
                    {
                        isColl = false;
                        break;
                    }
                }
                if (_dir == dir.down)
                {
                    if (collision.contacts[i].point.y > coll.bounds.center.y)
                    {
                        isColl = false;
                        break;
                    }
                }
                if (_dir == dir.right)
                {
                    if (collision.contacts[i].point.x < coll.bounds.center.x)
                    {
                        isColl = false;
                        break;
                    }
                }
            }

            if (isColl)
            {
                if (collision.transform.tag == "Player")
                {
                    animator.SetBool("isPop", true);
                    CharacterControl.instance.bounce(time, speed, _dir);
                }
            }
        }

        if (isOther)
        {
            animator.SetBool("isPop", true);
        }

        if(collision.transform.tag == "arms_player")  //被打击增加弹性
        {
            SR.color -= GameFunction.getColor(0, 0.25f, 0.25f,0);
            correctSR_Color();
            Behavior_beAttacked(collision.contacts[0].point, backSpeed);
            speed *= add_Sccale;
            animator.SetTrigger("beHit");
            if(!b_cooling)
            {
                StartCoroutine(cooling());
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "arms_player")  //被打击增加弹性
        {
            SR.color -= GameFunction.getColor(0, 0.25f, 0.25f, 0);
            correctSR_Color();
            speed *= add_Sccale;
            animator.SetTrigger("beHit");
            if (!b_cooling)
            {
                StartCoroutine(cooling());
            }
        }
    }

    public void changeAnimatorState()
    {
        animator.SetBool("isPop", false);
    }

    private bool b_cooling = false;
    IEnumerator cooling()
    {
        b_cooling = true;
        while (true)
        {
            if (SR.color.g < 0.99f)
            {
                SR.color += new Color(0, coolingSpeed * Time.deltaTime, coolingSpeed * Time.deltaTime, 0);
                correctSR_Color();
                int stage = (int)(SR.color.g / 0.25f);
                switch (stage)
                {
                    case 0:
                        speed = originSpeed * Mathf.Pow(add_Sccale, 4);
                        break;
                    case 1:
                        speed = originSpeed * Mathf.Pow(add_Sccale, 3);
                        break;
                    case 2:
                        speed = originSpeed * Mathf.Pow(add_Sccale, 2);
                        break;
                    case 3:
                        speed = originSpeed * Mathf.Pow(add_Sccale, 1);
                        break;
                    default:
                        speed = originSpeed;
                        break;
                }
            }
            else
            {
                b_cooling = false;
                speed = originSpeed;
                break;
            }
            yield return null;
        }
    }

    void correctSR_Color()  //纠正颜色数值
    {
        if(SR.color.g < 0.0)
        {
            SR.color = new Color(SR.color.r, 0, SR.color.b);
        }
        if (SR.color.g > 1.0)
        {
            SR.color = new Color(SR.color.r, 1.0f, SR.color.b);
        }
        if (SR.color.b < 0.0)
        {
            SR.color = new Color(SR.color.r, SR.color.g, 0.0f);
        }
        if (SR.color.g > 1.0)
        {
            SR.color = new Color(SR.color.r, SR.color.g, 1.0f);
        }
    }

    //玩家击中某个物体
    void Behavior_beAttacked(Vector2 pos, float backSpeed)
    {
        CharacterControl.instance.rig.AddForce(new Vector2(CharacterControl.instance.Dir == dir.left ? backSpeed : -backSpeed, 0.0f));  //后退
        GameObject t = CharacterObjectManager.instance.getHitPoint();   //打击动画
        t.SetActive(true);
        t.transform.position = pos;
        t.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
    }
}
