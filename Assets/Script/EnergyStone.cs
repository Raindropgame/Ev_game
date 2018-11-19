using UnityEngine;
using System.Collections;

public class EnergyStone : MonoBehaviour {

    public float Stone_move_Scale = 0;
    public float Stone_move_cycle = 0;
    public float Stone_scale_scale = 0;
    public float first_animation_duration = 0;
    public float second_animation_duration = 0;


    private GameObject BG, Stone,Halo,Energy,Particle;
    private bool isNone = false;
    private Vector3 originPos_Stone;
    private Vector3 originScale_Stone;
    private bool isTrigger = false;
    private Vector3 originPos;

	void Start () {
	    foreach(Transform t in GetComponentsInChildren<Transform>())
        {
            switch(t.name)
            {
                case "BG":
                    BG = t.gameObject;
                    break;
                case "Stone":
                    Stone = t.gameObject;
                    break;
                case "Halo":
                    Halo = t.gameObject;
                    break;
                case "Energy":
                    Energy = t.gameObject;
                    break;
                case "Particle":
                    Particle = t.gameObject;
                    break;
                default:
                    break;
            }
        }

        if (!isNone)
        {
            originPos_Stone = Stone.transform.position;
            originScale_Stone = Stone.transform.localScale;
            originPos = transform.position;
        }
        else
        {
            Particle.SetActive(false);
            Stone.SetActive(false);
            BG.GetComponent<SpriteRenderer>().color = Color.black;
        }
        Halo.SetActive(false);
	}
	
	void Update () {
	    if(!isNone)
        {
            Stone.transform.position = originPos_Stone + Vector3.up * Stone_move_Scale * Mathf.Sin(Time.time * 1 / Stone_move_cycle);
            Stone.transform.localScale = originScale_Stone + Vector3.left * Stone_scale_scale * Mathf.Sin(Time.time * 1 / Stone_move_cycle);
        }

        if(!isTrigger)
        {
            transform.position = originPos;
        }
        isTrigger = false;
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name.Length >= 6)
        {
            if (collision.name.Substring(0, 6).CompareTo("attack") == 0)
            {
                this.transform.position = originPos + (Vector3)Random.insideUnitCircle * 0.2f;
                isTrigger = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Length >= 6)
        {
            if (collision.name.Substring(0, 6).CompareTo("attack") == 0)
            {
                if (!isNone)
                {
                    StartCoroutine(IE_getEnergy());
                }
            }
        }
    }

    IEnumerator IE_getEnergy()
    {
        isNone = true;

        Halo.SetActive(true);
        Halo.transform.parent = null;
        Halo.GetComponent<TrailRenderer>().enabled = true;

        //第一段动画
        {
            float _time0 = 0;
            Vector3 _dir = Halo.transform.position - CharacterControl.instance.transform.position;
            Vector3 _dir_2 = Vector3.Cross(_dir, Vector3.back);
            _dir.z = 0;
            Vector3 p1, p2;
            p1 = Halo.transform.position + _dir * 0.9f;
            p2 = (CharacterControl.instance.transform.position + Halo.transform.position) / 2.0f + _dir_2 * 0.8f;
            p1.z = 0;
            p2.z = 0;
            while (_time0 < first_animation_duration)
            {
                _time0 += Time.deltaTime;
                float t = _time0 / first_animation_duration;
                Halo.transform.position = GameFunction.BezierLine(Halo.transform.position, CharacterControl.instance.transform.position + Vector3.up * 0.2f, p1, p2, t);

                yield return null;
            }
        }

        //第二段动画
        {
            Energy.SetActive(false);

            float _time0 = 0;
            Vector3 originScale_Halo = Halo.transform.localScale;
            SpriteRenderer SR_BG = BG.GetComponent<SpriteRenderer>();
            Color originColor_BG = SR_BG.color;
            SpriteRenderer SR_Stone = Stone.GetComponent<SpriteRenderer>();
            Color originColor_Stone = SR_Stone.color;
            SpriteRenderer SR_Halo = Halo.GetComponent<SpriteRenderer>();
            while(_time0 < second_animation_duration)
            {
                _time0 += Time.deltaTime;
                float t = _time0 / second_animation_duration;
                Halo.transform.localScale = Vector3.Lerp(originScale_Halo, Vector3.one * 10f, t);
                SR_Halo.color = Color.Lerp(SR_Halo.color, new Color(1, 1, 1, 0), t);
                SR_BG.color = Color.Lerp(originColor_BG, Color.black, t);
                SR_Stone.color = Color.Lerp(originColor_Stone, new Color(1, 1, 1, 0), t);

                yield return null;
            }
            SR_BG.sortingOrder = 0;
            Stone.SetActive(false);
            Halo.SetActive(false);
            Particle.SetActive(false);
        }
        Bag.getInstance().AddArmsFragment(1);

    }
}
