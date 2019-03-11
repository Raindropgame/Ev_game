using UnityEngine;
using System.Collections;

public class platform_jump : MonoBehaviour {

    public ParticleSystem particle;
    public SpriteRenderer Halo;
    public float restTime;
    public Color disableColor;
    public float scale;
    public float speed;
    public Light _light;
    public float nearRange = 1;
    public float maxDistanceDelta;

    private bool Rest = false;
    private SpriteRenderer SR;
    private Vector3 originPos;  //原点
    private float randomTime1,randomTime2;  //随机的预先时间
    private float light_rest_range = 4,origin_range;
    private bool isCharacterIn = false;

	// Use this for initialization
	void Start () {
        origin_range = _light.range;
        originPos = transform.position;
        SR = GetComponent<SpriteRenderer>();
        Random.InitState((int)transform.position.x);
        randomTime1 = Random.value * 10;
        randomTime2 = Random.value * 10;
    }
	
	// Update is called once per frame
	void Update () {

        if(((Vector2)(originPos - CharacterControl.instance.transform.position)).sqrMagnitude < Mathf.Pow(nearRange,2) && !Rest) //是否在靠近的范围内
        {
            transform.position = Vector2.MoveTowards(transform.position, CharacterControl.instance.transform.position, maxDistanceDelta);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(scale * Mathf.Sin(speed * Time.time + randomTime1) + originPos.x, scale * Mathf.Sin(speed * Time.time + randomTime2) + originPos.y), maxDistanceDelta * 0.6f);
        }

        if(isCharacterIn)
        {
            if (!Rest)
            {
                if (MyInput.instance.isGetJumpDown())
                {
                    CharacterControl.instance._jumpTimes--;
                    Rest = true;
                    particle.gameObject.SetActive(true);
                    Invoke("reset", restTime);
                    SR.color = disableColor;
                    _light.range = light_rest_range;
                }
            }
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, originPos.z);   //纠正Z值
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {         
            isCharacterIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            isCharacterIn = false;
        }
    }

    void reset()
    {
        particle.gameObject.SetActive(false);
        StartCoroutine(halo());
    }

    IEnumerator halo()
    {
        float alpha_start = 0.7f;
        float duration = 0.5f, _time1 = 0;
        while(true)
        {
            _time1 += Time.deltaTime;
            Halo.color = new Color(Halo.color.r, Halo.color.g, Halo.color.b, Mathf.Lerp(alpha_start, 1, _time1 / duration));
            Halo.gameObject.transform.localScale = Vector3.Lerp(Vector3.one * 3, Vector3.zero, _time1 / duration);
            _light.range = Mathf.Lerp(light_rest_range, origin_range, _time1 / duration);
            if(_time1 > duration)
            {
                break;
            }
            yield return null;
        }
        Rest = false;
        SR.color = Color.white;
    }
}
