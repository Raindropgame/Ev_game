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

    private bool Rest = false;
    private SpriteRenderer SR;
    private Vector3 originPos;  //原点
    private float randomTime1,randomTime2;  //随机的预先时间
    private float light_rest_range = 4,origin_range;

	// Use this for initialization
	void Start () {
        origin_range = _light.range;
        originPos = transform.position;
        SR = GetComponent<SpriteRenderer>();
        randomTime1 = Random.Range(1, 100);
        randomTime2 = Random.Range(1, 100);
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(scale * Mathf.Sin(speed * Time.time + randomTime1) + originPos.x, scale * Mathf.Sin(speed * Time.time + randomTime2) + originPos.y, originPos.z);
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !Rest)
        {
            if (Input.GetKeyDown(KeyCode.Space))
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
