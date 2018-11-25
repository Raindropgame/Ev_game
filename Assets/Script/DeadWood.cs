using UnityEngine;
using System.Collections;

public class DeadWood : MonoBehaviour {

    public Element ElementTrigger;
    public int FireNum = 5;
    public float BurnTime = 0.5f;
    public SpriteRenderer[] SRs;
    public bool isInRain = false;

    private bool isBurning = false;
    private bool isBurningOver = false;
    private BoxCollider2D BoxColl;
    private bool isInShelter = false;



    private void Start()
    {
        BoxColl = GameFunction.GetGameObjectInChildrenByName(this.gameObject, "Collider").GetComponent<BoxCollider2D>();
    }

    void Update () {
        if (!isBurning)
        {
            if (ElementTrigger.isContainElement(Attribute.fire))
            {
                StartCoroutine(IE_Burning());
            }
        }

        isInRain = false;
        if(!isInShelter)
        {
            if(Weather.instance.isRain())
            {
                isInRain = true;
            }
        }
	}

    IEnumerator IE_Burning()
    {
        isBurning = true;

        bool isNormal = true;  //是否被熄灭
        GameObject Fire = Resources.Load<GameObject>("Fire");

        GameObject[] t = new GameObject[FireNum];
        Vector3[] originScale_fire = new Vector3[t.Length];
        Vector3 t_vec3 = Vector3.zero;
        for (int i = 0; i < t.Length; i++)
        {
            t_vec3.x = Random.Range(-BoxColl.bounds.extents.x, BoxColl.bounds.extents.x);
            t_vec3.y = Random.Range(-BoxColl.bounds.extents.y, BoxColl.bounds.extents.y);
            t_vec3.z = -3;
            t[i] = Instantiate(Fire, position: BoxColl.bounds.center + t_vec3, rotation: Quaternion.Euler(Vector3.zero)) as GameObject;
            t[i].transform.SetParent(this.transform, true);
            t[i].transform.localScale = t[i].transform.localScale * Random.Range(0.6f, 1.2f);
            originScale_fire[i] = t[i].transform.localScale;
            t[i].transform.localScale = Vector3.zero;
        }

        //第一段动画
        {
            const float first_animation_duration = 0.2f;
            float _time0 = 0;
            while(_time0 < first_animation_duration)
            {
                _time0 += Time.deltaTime;
                float _t = _time0 / first_animation_duration;
                
                for(int i = 0;i<t.Length;i++)
                {
                    t[i].transform.localScale = Vector3.Lerp(Vector3.zero, originScale_fire[i], _t);
                }

                yield return null;
            }
        }

        //第二段动画
        {
            float _time0 = 0;
            while(_time0 < BurnTime)  //火焰燃烧
            {
                _time0 += Time.deltaTime;

                if(ElementTrigger.isContainElement(Attribute.ice) || isInRain)
                {
                    isNormal = false;
                    break;
                }

                yield return null;
            }
        }

        //第三段动画
        {
            MaterialPropertyBlock[] t_MB = new MaterialPropertyBlock[SRs.Length];
            for(int i = 0;i<t_MB.Length;i++)
            {
                t_MB[i] = new MaterialPropertyBlock();
                t_MB[i].SetTexture("_MainTex", SRs[i].sprite.texture);
                t_MB[i].SetFloat("_BurnScale", 0);
            }
            const float third_animation_duration = 0.5f;
            float _time0 = 0;
            bool isSmoke = false;
            while(_time0 < third_animation_duration)
            {
                _time0 += Time.deltaTime;
                float _t = _time0 / third_animation_duration;
                for(int i = 0;i<t.Length;i++)
                {
                    t[i].transform.localScale = Vector3.Lerp(originScale_fire[i], Vector3.zero, _t);
                }

                if (isNormal)
                {
                    for (int i = 0; i < SRs.Length; i++)
                    {
                        t_MB[i].SetFloat("_BurnScale", Mathf.Lerp(0, 1, _t));
                        SRs[i].SetPropertyBlock(t_MB[i]);
                    }

                    if (!isSmoke)
                    {
                        if (_t > 0.7f)  //出现烟
                        {
                            isSmoke = true;

                            GameObject t_smoke_res = Resources.Load<GameObject>("Smoke");
                            GameObject t_smoke = Instantiate(t_smoke_res, position: BoxColl.bounds.center, rotation: Quaternion.Euler(Vector3.zero)) as GameObject;
                            ParticleSystem.ShapeModule t_smoke_shape = t_smoke.GetComponent<ParticleSystem>().shape;
                            t_smoke_shape.box = new Vector3(BoxColl.bounds.size.x * 2, BoxColl.bounds.size.y, 0);
                            t_smoke_res = null;
                            Resources.UnloadUnusedAssets();
                        }
                    }
                }



                yield return null;
            }
        }

        //是否被熄灭
        if (isNormal)
        {
            Destroy(this.gameObject);
            isBurningOver = true;
        }
        else
        {
            for(int i = 0;i<t.Length;i++)
            {
                Destroy(t[i]);
            }
        }
        isBurning = false;
        Fire = null;
        Resources.UnloadUnusedAssets();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(LayerMask.LayerToName(collision.gameObject.layer).CompareTo("shelter") == 0)
        {
            isInShelter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer).CompareTo("shelter") == 0)
        {
            isInShelter = false;
        }
    }
}
