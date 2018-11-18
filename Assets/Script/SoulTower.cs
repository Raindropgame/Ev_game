using UnityEngine;
using System.Collections;

public class SoulTower : MonoBehaviour {

    [Header("动画参数")]
    public float shakeScale;
    public float first_animation_waitTime;
    public float first_animation_height;
    public float first_animation_duration;
    public float second_animation_duration;
    public float third_animation_duration;

    private GameObject Soul, Halo, Bloom, Eye, Eye2,Particle,Aperture;
    private Animator animator;
    private Vector3 originPos;
    private bool isTrigger = false;
    private bool isNone = false;

	void Start () {      
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            switch(t.name)
            {
                case "Soul":
                    Soul = t.gameObject;
                    break;
                case "Halo":
                    Halo = t.gameObject;
                    break;
                case "Bloom":
                    Bloom = t.gameObject;
                    break;
                case "Eye":
                    Eye = t.gameObject;
                    break;
                case "Eye2":
                    Eye2 = t.gameObject;
                    break;
                case "Particle":
                    Particle = t.gameObject;
                    break;
                case "Aperture":
                    Aperture = t.gameObject;
                    t.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
        animator = GetComponent<Animator>();
        originPos = transform.position;

        if(isNone)
        {
            Halo.SetActive(false);
            Eye.GetComponent<SpriteRenderer>().color = Color.white * 0.3f;
            Eye2.GetComponent<SpriteRenderer>().color = Color.white * 0.3f;
            Particle.SetActive(false);

            this.enabled = false;
        }
	}

    private void Update()
    {
        if (!isTrigger)
        {
            this.transform.position = originPos;
        }
        isTrigger = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name.Substring(0,6).CompareTo("attack") == 0)  //被击中
        {
            if (!isAnimation && !isNone)
            {
                StartCoroutine(IE_getNewSoul());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.name.Substring(0, 6).CompareTo("attack") == 0 && this.enabled)
        {
            this.transform.position = originPos + (Vector3)Random.insideUnitCircle * shakeScale;
            isTrigger = true;
        }
    }

    private bool isAnimation = false;
    IEnumerator IE_getNewSoul()  //获得新灵魂动画
    {


        isAnimation = true;
        CharacterControl.instance.setInputNone();
        yield return new WaitForSeconds(first_animation_waitTime);
        animator.enabled = false;
        Halo.GetComponent<TrailRenderer>().enabled = true;
        Particle.SetActive(false);
        Aperture.SetActive(true);

        {
            Vector3 targetPos = Halo.transform.position + new Vector3(0, first_animation_height, 0);
            Vector3 t_originPos = Halo.transform.position;
            Vector3 t_vec3 = Vector3.zero;
            SpriteRenderer t_SR = Aperture.GetComponent<SpriteRenderer>();
            float _time0 = 0, _time1 = 0;
            const float Aperture_duration = 0.4f;
            while (_time0 < first_animation_duration)  //第一段动画
            {
                if (_time1 > 0.2f * Aperture_duration)
                {
                    _time0 += (Time.deltaTime * ((first_animation_duration * 1.6f - _time0) / first_animation_duration));
                }

                _time1 += Time.deltaTime;
                t_vec3 = Vector3.Lerp(t_originPos, targetPos, _time0 / first_animation_duration);
                t_vec3.x = Mathf.Sin(_time0 * 3) * 0.7f + originPos.x;
                Halo.transform.position = t_vec3;

                if (_time1 < Aperture_duration * 1.1f)  //光圈扩大后开始动画
                {
                    float timeScale = _time1 / Aperture_duration;
                    Aperture.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 20, timeScale);
                    t_SR.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), timeScale);
                }

                yield return null;
            }
            Aperture.SetActive(false);
        }

        //第二段动画
        {
            float _time0 = 0;
            Vector3 t_originPos = Halo.transform.position;
            Vector3 p1, p2;
            Vector3 _dir = Halo.transform.position - CharacterControl.instance.transform.position;
            Vector3 _dir_2 = Vector3.Cross(_dir, Vector3.back);
            Halo.GetComponent<TrailRenderer>().time *= 0.3f;
            _dir.z = 0;
            _dir_2.z = 0;
            p1 = Halo.transform.position + _dir * 0.8f;
            p2 = (CharacterControl.instance.transform.position + Halo.transform.position) / 2.0f + _dir_2 * 0.7f;
            p1.z = 0;
            p2.z = 0;
            while (_time0 < second_animation_duration)
            {
                _time0 += (Time.deltaTime * (_time0 + 0.4f * second_animation_duration) / second_animation_duration);
                float t = _time0 / second_animation_duration;
                Halo.transform.position = BezierLine(t_originPos, CharacterControl.instance.transform.position, p1, p2, t);

                yield return null;
            }            
        }

        //第三段动画
        {
            Soul.SetActive(false);

            float _time0 = 0;
            SpriteRenderer t_SR = Halo.GetComponent<SpriteRenderer>();
            SpriteRenderer bloom_SR = Bloom.GetComponent<SpriteRenderer>();
            while(_time0 < third_animation_duration)
            {
                _time0 += Time.deltaTime;
                float t = _time0 / third_animation_duration;
                Halo.transform.localScale = Vector3.Lerp(Halo.transform.localScale, Vector3.one * 30f, t);
                t_SR.color = Color.Lerp(t_SR.color, new Color(1, 1, 1, 0), t);
                bloom_SR.color = Color.Lerp(bloom_SR.color, new Color(1, 1, 1, 0), t);

                yield return null;
            }
            Halo.SetActive(false);
            Eye.GetComponent<SpriteRenderer>().color = Color.white * 0.3f;
            Eye2.GetComponent<SpriteRenderer>().color = Color.white * 0.3f;
        }

        CharacterAttribute.GetInstance().add_MaxHP(1);
        CharacterControl.instance.getInput();

        isAnimation = false;
        this.enabled = false;
        isNone = true;
    }

    Vector3 BezierLine(Vector3 from,Vector3 to,Vector3 p1,Vector3 p2,float t)
    {
        Vector3 t1, t2, t3,t4,t5;
        t1 = Vector3.Lerp(from, p1, t);
        t2 = Vector3.Lerp(p1, p2, t);
        t3 = Vector3.Lerp(p2, to, t);

        t4 = Vector3.Lerp(t1, t2,t);
        t5 = Vector3.Lerp(t2, t3, t);

        return Vector3.Lerp(t4, t5, t);
    }

}
