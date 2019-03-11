using UnityEngine;
using System.Collections;

public class Grass : MonoBehaviour {

    public Element elementTrigger;
    public float maxBurnScale = 1;
    public Color burnColor,unBurnColor;
    public float burnTime = 2;

    private bool isSway = false;
    private Animator animator;
    private bool isBurn = false;
    private float burnedTime = 0; //已经燃烧的时间
    private SpriteRenderer SR;
    private bool isInRain = false;
    private bool isInShelter = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        SR = GetComponent<SpriteRenderer>();
        isInShelter = checkIsInShelter();
    }

    private void Update()
    {
        if (!isBurn)
        {
            if (elementTrigger.isContainElement(Attribute.fire))
            {
                isBurn = true;
                StartCoroutine(IE_burn());
            }
        }

        //是否在雨中
        if (Weather.instance.isRain() && !isInShelter)
            isInRain = true;
        else
            isInRain = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isSway)
        {
            dir _dir;
            if (collision.transform.position.x > transform.position.x)
                _dir = dir.left;
            else
                _dir = dir.right;

            animator.SetTrigger("sway");
            isSway = true;
            animator.SetInteger("dir", _dir == dir.left ? 1 : -1);
        }
    }

    /// <summary>
    /// 燃烧
    /// </summary>
    /// <returns>null</returns>
    IEnumerator IE_burn()
    {
        GameFunction.t_Vector3 = transform.position;
        GameFunction.t_Vector3.z -= 0.01f;
        GameObject fire = Instantiate(ResourcesManagement.getInstance().getResources("Fire"), position: GameFunction.t_Vector3, rotation: Quaternion.Euler(Vector3.zero)) as GameObject;
        Bounds bounds_fire = fire.GetComponentInChildren<BoxCollider2D>().bounds;
        Vector3 originScale_fire;

        //适应图片比例
        float scale_x = SR.bounds.size.x / bounds_fire.size.x;
        float scale_y = SR.bounds.size.y * 0.6f / bounds_fire.size.y;
        GameFunction.t_Vector3.x = scale_x * fire.transform.localScale.x;
        GameFunction.t_Vector3.y = scale_y * fire.transform.localScale.y;
        GameFunction.t_Vector3.z = transform.localScale.z;
        originScale_fire = GameFunction.t_Vector3;

        const float time_show = 0.4f;
        const float time_end = 0.2f;
        float timer = 0;
        //出现
        while(timer < time_show)
        {
            timer += Time.deltaTime;
            float t = timer / time_show;

            fire.transform.localScale = Vector3.Lerp(Vector3.zero, originScale_fire, t);
            yield return null;
        }

        timer = 0;
        MaterialPropertyBlock MB = new MaterialPropertyBlock();
        bool isExtinguish = false;
        SR.GetPropertyBlock(MB);
        MB.SetColor("_BurnColor", burnColor);
        //燃烧持续
        while(timer < burnTime)
        {
            timer += Time.deltaTime;

            burnedTime += Time.deltaTime;
            MB.SetFloat("_BurnScale", Mathf.Lerp(0,maxBurnScale,burnedTime / burnTime));
            SR.SetPropertyBlock(MB);

            //熄灭
            if(elementTrigger.isContainElement(Attribute.ice) || isInRain)
            {
                isExtinguish = true;
                break;
            }
            yield return null;
        }

        timer = 0;
        //结束
        while(timer < time_end)
        {
            timer += Time.deltaTime;
            float t = timer / time_end;

            fire.transform.localScale = Vector3.Lerp(originScale_fire, Vector3.zero, t);
            if(isExtinguish)
            {
                MB.SetColor("_BurnColor", Color.Lerp(burnColor, unBurnColor, t));
                SR.SetPropertyBlock(MB);
            }

            yield return null;
        }

        Instantiate(ResourcesManagement.getInstance().getResources("Smoke"),position:fire.transform.position,rotation:Quaternion.Euler(Vector3.zero));
        if(isExtinguish)
        {
            Destroy(fire);
            fire = null;
            yield return new WaitForSeconds(0.3f);
            isBurn = false;
        }
        else
        {
            Destroy(this.gameObject);
            Destroy(fire);
            fire = null;
        }
    }

    public void Finish()
    {
        isSway = false;
    }

    bool checkIsInShelter()
    {
        int mask = 1 << LayerMask.NameToLayer("shelter");
        RaycastHit2D hitPoint = Physics2D.Raycast(SR.bounds.center, Vector2.down, 0.01f, mask);
        if(hitPoint.transform != null)
        {
            return true;
        }
        return false;
    }

}
