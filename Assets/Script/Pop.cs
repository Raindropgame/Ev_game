using UnityEngine;
using System.Collections;

public class Pop : MonoBehaviour {

    public float speed, time;
    public dir _dir;
    public bool isOther = false;

    private Animator animator;
    private Collider2D coll;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if(animator == null)
        {
            animator = transform.parent.GetComponentInChildren<Animator>();
        }
        coll = GetComponent<Collider2D>();
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

    }

    public void changeAnimatorState()
    {
        animator.SetBool("isPop", false);
    }
}
