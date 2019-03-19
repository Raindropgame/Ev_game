using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

    static public ArrayList portalArray = new ArrayList();
    static public void relink()
    {
        for(int i = 0;i<portalArray.Count;i++)
        {
            ((Portal)portalArray[i]).findLinkPortal();
        }
    }

    static private ParticleSystem m_Effect_portal_show = null;
    static public ParticleSystem Effect_portal_show
    {
        get
        {
            if(m_Effect_portal_show == null)
            {
                GameObject t = Instantiate(((GameObject)ResourcesManagement.getInstance().getResources<GameObject>("Effect_portal_show")), position: Vector3.zero, rotation: Quaternion.Euler(Vector3.zero)) as GameObject;
                m_Effect_portal_show = t.GetComponent<ParticleSystem>();
            }
            return m_Effect_portal_show;
        }
    }

    public string[] sender_tag;
    [Header("动画属性")]
    public SpriteRenderer SR_halo;
    public Color[] color_halo;
    public float scale_halo;
    public LineRenderer LR;

    private Portal linkPortal = null;
    private bool isEnable = false;
    private Vector3 originPos;
    private Vector3 originScale_halo;
    private bool isTranfer = false;

    private void Awake()
    {
        portalArray.Add(GetComponent<Portal>());
    }

    private void Start()
    {
        findLinkPortal();
        isEnable = true;

        originPos = transform.position;
        originScale_halo = SR_halo.transform.localScale;
        StartCoroutine(IE_idle());
    }

    public void findLinkPortal()
    {
        int linkIndex = 0;
        float minLength = 99999;
        Portal t;
        for(int i = 0;i<portalArray.Count;i++)
        {
            t = portalArray[i] as Portal;
            //排除自己
            if(!t.Equals(this))
            {
                float length = ((Vector2)(t.transform.position - transform.position)).sqrMagnitude;
                if (length < minLength)
                {
                    linkIndex = i;
                    minLength = length;
                }
            }

        }

        linkPortal = portalArray[linkIndex] as Portal;

        if(portalArray.Count > 1)
        {
            const float offset = 0.9f;
            GameFunction.t_Vector3 = transform.position + Vector3.down * offset;
            GameFunction.t_Vector3.z = transform.position.z + 0.5f;
            LR.SetPosition(0, GameFunction.t_Vector3);
            GameFunction.t_Vector3 = linkPortal.transform.position + Vector3.up * offset;
            GameFunction.t_Vector3.z = transform.position.z + 0.5f;
            LR.SetPosition(1, GameFunction.t_Vector3);
        }
    }

    void transfer(Transform t)
    {
        Effect_portal_show.transform.position = t.transform.position;
        Effect_portal_show.Play();
        StartCoroutine(IE_sendSender());
        GameFunction.t_Vector3 = linkPortal.transform.position;
        GameFunction.t_Vector3.z = t.position.z;
        t.position = GameFunction.t_Vector3;
        linkPortal.getSender();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEnable)
        {
            for(int i = 0;i<sender_tag.Length;i++)
            {
                if (collision.tag.CompareTo(sender_tag[i]) == 0)
                {
                    try
                    {
                        //不为刀
                        if (collision.transform.name.Substring(0, 6).CompareTo("attack") == 0)
                            continue;
                    }
                    catch
                    {

                    }
                    isEnable = false;
                    StartCoroutine(cool());
                    transfer(collision.transform);
                }
            }
        }
    }

    public void getSender()
    {
        StartCoroutine(IE_getSender());
        isEnable = false;
        StartCoroutine(cool());
    }

    IEnumerator cool()
    {
        const float coolTime = 0.4f;
        yield return new WaitForSeconds(coolTime);
        isEnable = true;
    }

    IEnumerator IE_idle()
    {
        float timer = Random.Range(0,10);
        float scale = 0.2f;
        while(true)
        {
            timer += Time.deltaTime;

            transform.position = originPos + Vector3.up * Mathf.Sin(timer) * scale;
            SR_halo.color = Color.Lerp(color_halo[0], color_halo[1], 0.5f * Mathf.Sin(timer * 2.0f) + 0.5f);
            if(!isTranfer)
                SR_halo.transform.localScale = originScale_halo * Mathf.Lerp(1, scale_halo, 0.5f * Mathf.Cos(timer * 1.5f) + 0.5f);

            yield return null;
        }
    }

    IEnumerator IE_sendSender()
    {
        isTranfer = true;

        const float duration = 0.3f;
        float timer = 0;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            SR_halo.transform.localScale = Vector3.Lerp(SR_halo.transform.localScale, Vector3.zero, t);

            yield return null;
        }

        timer = 0;
        while (timer < (duration * 1.5f))
        {
            timer += Time.deltaTime;
            float t = timer / (duration * 1.5f);

            SR_halo.transform.localScale = Vector3.Lerp(originScale_halo, Vector3.zero, 1 - t);

            yield return null;
        }

        isTranfer = false;
    }

    IEnumerator IE_getSender()
    {
        isTranfer = true;

        yield return null;
        Effect_portal_show.transform.position = transform.position;
        Effect_portal_show.Play();

        const float duration = 0.45f;
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            SR_halo.transform.localScale = Vector3.Lerp(Vector3.zero, originScale_halo, t);

            yield return null;
        }

        isTranfer = false;
    }

    private void OnDestroy()
    {
        portalArray.Remove(this);
    }
}
