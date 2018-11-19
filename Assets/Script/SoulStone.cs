using UnityEngine;
using System.Collections;

public class SoulStone : MonoBehaviour {

    public float shakeScale = 0.3f;
    public float WanderScale;
    public float WanderCycle;
    public float animation_duration;
    public float duration_expand;

    private GameObject Particle, Halo,Soul;
    private bool isNone = false;
    private bool isTrigger = false;
    private Vector3 originPos;
    private Vector3 originPos_Halo;

	void Start () {
	    foreach(Transform t in GetComponentsInChildren<Transform>())
        {
            switch(t.name)
            {
                case "Particle":
                    Particle = t.gameObject;
                    break;
                case "Halo":
                    Halo = t.gameObject;
                    break;
                case "Soul":
                    Soul = t.gameObject;
                    break;
                default:
                    break;
            }
        }

        originPos = transform.position;
        originPos_Halo = Halo.transform.position;

        if(isNone)
        {
            Halo.SetActive(false);
            Particle.SetActive(false);
        }
	}
	
	void Update () {
        if (!isTrigger)
        {
            this.transform.position = originPos;
        }
        isTrigger = false;

        if(!isNone)
        {
            Halo.transform.position = originPos_Halo + Vector3.up * Mathf.Sin(Time.time * 1 / WanderCycle) * WanderScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Length >= 6)
        {
            if (collision.name.Substring(0, 6).CompareTo("attack") == 0)  //被击中
            {
                if (!isNone)
                {
                    StartCoroutine(IE_getSoul());
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name.Length >= 6)
        {
            if (collision.name.Substring(0, 6).CompareTo("attack") == 0 && this.enabled)
            {
                this.transform.position = originPos + (Vector3)Random.insideUnitCircle * shakeScale;
                isTrigger = true;
            }
        }
    }

    IEnumerator IE_getSoul()
    {
        isNone = true;

        Halo.GetComponent<TrailRenderer>().enabled = true;

        float _time0 = 0;
        Vector3 t_originPos_Halo = Halo.transform.position;
        Vector3 p1, p2;
        Vector3 _dir = Halo.transform.position - CharacterControl.instance.transform.position;
        Vector3 _dir_2 = Vector3.Cross(_dir, Vector3.back);
        _dir.z = 0;
        _dir_2.z = 0;
        p1 = Halo.transform.position + _dir * 2f;
        p2 = (CharacterControl.instance.transform.position + Halo.transform.position) / 2.0f + _dir_2 * 1f;
        p1.z = 0;
        p2.z = 0;
        while (_time0 < animation_duration)
        {
            _time0 += Time.deltaTime;
            float t = _time0 / animation_duration;
            Halo.transform.position = BezierLine(t_originPos_Halo, CharacterControl.instance.transform.position, p1, p2, t);

            yield return null;
        }
        CharacterAttribute.GetInstance().add_HP(1);

        _time0 = 0;
        Soul.SetActive(false);
        SpriteRenderer t_SR = Halo.GetComponent<SpriteRenderer>();
        while(_time0 < duration_expand)
        {
            _time0 += Time.deltaTime;
            float t = _time0 / duration_expand;
            Halo.transform.localScale = Vector3.Lerp(Halo.transform.localScale, Vector3.one * 6f, t);
            t_SR.color = Color.Lerp(t_SR.color, new Color(1, 1, 1, 0), t);

            yield return null;
        }

        Particle.SetActive(false);
        Halo.SetActive(false);
    }

    Vector3 BezierLine(Vector3 from, Vector3 to, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 t1, t2, t3, t4, t5;
        t1 = Vector3.Lerp(from, p1, t);
        t2 = Vector3.Lerp(p1, p2, t);
        t3 = Vector3.Lerp(p2, to, t);

        t4 = Vector3.Lerp(t1, t2, t);
        t5 = Vector3.Lerp(t2, t3, t);

        return Vector3.Lerp(t4, t5, t);
    }
}
