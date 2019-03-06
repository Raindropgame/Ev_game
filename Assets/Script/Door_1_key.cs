using UnityEngine;
using System.Collections;

public class Door_1_key : MonoBehaviour {

    public float speed;
    public Vector3 offset;
    public Transform bloom;
    public Transform effect;

    private bool isAttract = false;
    private Vector3 originPos;
    private Vector3 targetPos;
    private float timer = 0;
    private bool isStop = false;
    private BoxCollider2D boxColl;
    private CircleCollider2D circleColl;
    private Vector3 originScale_bloom;
    private SpriteRenderer SR_bloom;
    private Color originColor_bloom;
    private Coroutine bloomCor = null;
    private bool isDisenble = false;
    private Vector3 originScale_effect;

    private void Awake()
    {
        originPos = this.transform.position;
        CharacterControl.Event_hurt += back;
    }

    private void Start()
    {
        boxColl = GetComponent<BoxCollider2D>();
        circleColl = GetComponent<CircleCollider2D>();

        boxColl.enabled = true;
        circleColl.enabled = false;
        originScale_bloom = bloom.localScale;
        bloom.localScale = Vector3.zero;
        SR_bloom = bloom.GetComponent<SpriteRenderer>();
        originColor_bloom = SR_bloom.color;
        originScale_effect = effect.localScale;
    }

    private void FixedUpdate()
    {
        if (!isDisenble)
        {
            updatePos();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //取得钥匙
        if(collision.tag.CompareTo("Player") == 0)
        {
            if (!CharacterControl.instance.isGetDoor1_Key)
            {
                isAttract = true;
                boxColl.enabled = false;
                circleColl.enabled = true;
                CharacterControl.instance.isGetDoor1_Key = true;
                StartCoroutine(IE_bloom_animation(true));
            }
        }

        //解锁
        if(collision.tag.CompareTo("Door_1") == 0)
        {
            if(isAttract)
            {
                isDisenble = true;
                StartCoroutine(IE_destory(collision.gameObject));
                isAttract = false;
                CharacterControl.instance.isGetDoor1_Key = false;
            }
        }
    }

    void updatePos()
    {
        if (isAttract)
        {
            if (((Vector2)(this.transform.position - CharacterControl.instance.transform.position - offset)).sqrMagnitude > Mathf.Sqrt(0.1f) && !isStop)
            {
                this.transform.position = Vector2.MoveTowards(this.transform.position, CharacterControl.instance.transform.position + offset, speed * Time.deltaTime);

                targetPos = this.transform.position;
                timer = 0;
            }
            else
            {
                isStop = true;
            }

            //闲置
            if (isStop)
            {
                timer += Time.deltaTime;
                transform.position = targetPos + 0.2f * Mathf.Sin(timer * 1.5f) * Vector3.up;
                if (((Vector2)(this.transform.position - CharacterControl.instance.transform.position - offset)).sqrMagnitude > Mathf.Sqrt(0.5f))
                {
                    isStop = false;
                }
            }
        }
        else
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, originPos, speed * Time.deltaTime * 3);
            if(((Vector2)(this.transform.position - originPos)).sqrMagnitude < Mathf.Sqrt(0.2f))
            {
                boxColl.enabled = true;
                targetPos = originPos;
            }
        }
    }

    //玩家受伤回到原处
    void back()
    {
        if (isAttract)
        {
            CharacterControl.instance.isGetDoor1_Key = false;
            isAttract = false;
            circleColl.enabled = false;
            StartCoroutine(IE_bloom_animation(false));
        }
    }

    private void OnDestroy()
    {
        CharacterControl.Event_hurt -= back;
    }

    IEnumerator IE_destory(GameObject Door1)
    {
        Door_1 t_Door_1 = Door1.GetComponent<Door_1>();

        const float duration = 2f;
        float timer = 0;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            transform.position = Vector3.Lerp(transform.position, t_Door_1.Stone_trans.position + Vector3.back * 0.1f, t * 0.8f);
            effect.localScale = Vector3.Lerp(effect.localScale, Vector3.zero, t);

            yield return null;
        }

        Instantiate(ResourcesManagement.getInstance().getResources("Effect_expand"), position: transform.position, rotation: Quaternion.Euler(Vector3.zero));  //特效

        t_Door_1.Unlock();  //解锁
        CharacterControl.instance.isGetDoor1_Key = false;
        Destroy(this.gameObject);
    }

    IEnumerator IE_bloom_animation(bool isShow)
    {
        const float duration = 0.2f;

        float Timer = 0;
        while (Timer < duration)
        {
            Timer += Time.deltaTime;
            float t = Timer / duration;

            if (isShow)
            {
                bloom.localScale = Vector3.Lerp(Vector3.zero, originScale_bloom, t);
                effect.localScale = Vector3.Lerp(effect.localScale, originScale_effect * 0.6f,t);
            }
            else
            {
                bloom.localScale = Vector3.Lerp(originScale_bloom, Vector3.zero, t);
                effect.localScale = Vector3.Lerp(effect.localScale, originScale_effect,t);
            }

            yield return null;
        }

        if (isShow)
        {
            if (bloomCor == null)
            {
                bloomCor = StartCoroutine(IE_bloom());
            }
        }
        else
        {
            StopCoroutine(bloomCor);
            bloomCor = null;
        }
    }

    IEnumerator IE_bloom()
    {
        while(true)
        {
            float t = 0.5f * Mathf.Sin(Time.time * 3) + 0.5f;
            SR_bloom.color = Color.Lerp(originColor_bloom, GameFunction.Transparent, t);
            yield return null;
        }
    }
}
