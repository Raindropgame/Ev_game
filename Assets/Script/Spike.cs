using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour {

    public int damage;
    public float waitTime;
    public float showTime;
    public float duration;
    public Transform spike;
    public float distance;
    public BoxCollider2D Coll;

    private bool isShow = false;
    private bool isHurt = false;

	IEnumerator IE_show()
    {
        isShow = true;

        float timer_1 = 0;
        const float offset = 0.1f;
        while(timer_1 < waitTime)
        {
            timer_1 += Time.deltaTime;

            spike.localPosition = Random.insideUnitCircle * offset;
            yield return null;
        }

        isHurt = true;
        timer_1 = 0;
        while (timer_1 < waitTime)
        {
            timer_1 += Time.deltaTime;
            float t = timer_1 / showTime;

            spike.localPosition = Vector2.Lerp(Vector2.zero, Vector2.up * distance, t);
            yield return null;
        }

        yield return new WaitForSeconds(duration);
        isHurt = false;

        timer_1 = 0;
        while (timer_1 < waitTime)
        {
            timer_1 += Time.deltaTime;
            float t = timer_1 / showTime;

            spike.localPosition = Vector2.Lerp(Vector2.up * distance,Vector2.zero ,t);
            yield return null;
        }

        isShow = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(isShow)  //未隐藏时
        {
            if (isHurt)
            {
                //对玩家作用
                if (collision.tag.CompareTo("Player") == 0)
                {
                    CharacterControl.instance.hurt(damage, Attribute.normal, Coll.bounds.center);
                }
                //对敌人作用
                else if (collision.tag.CompareTo("enemy") == 0)
                {
                    CharacterObjectManager.instance.sendHurt_other(damage, Attribute.normal, collision.gameObject.GetInstanceID(), Coll.bounds.center);
                }
            }
        }
        else    //隐藏时
        {
            if (collision.tag.CompareTo("Player") == 0)
            {
                isShow = true;
                StartCoroutine(IE_show());
            }
        }
    }


}
