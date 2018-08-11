using UnityEngine;
using System.Collections;

public class stone : MonoBehaviour
{

    [Header("雨天减少的摩擦力度")]
    public float Scale = 0.6f;
    [Header("弹开玩家")]
    public float reactionForce = 2000;
    public float speed,time;

    private Rigidbody2D rig;
    private float drag, mass;
    private bool isFrozen = false;
    private SpriteRenderer SR;

    // Use this for initialization
    void Start()
    {
        CharacterObjectManager._sendHurt += getHurt;  //受伤消息(来自玩家)注册
        rig = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        drag = rig.drag;
        mass = rig.mass;
    }

    private void OnDestroy()
    {
        CharacterObjectManager._sendHurt -= getHurt;
    }

    private void FixedUpdate()
    {
        if (WeatherData.getIntance().currentWeather == weather.Rain || WeatherData.getIntance().currentWeather == weather.RainAndThunder)   //下雨减少摩擦
        {
            rig.drag = Scale * drag;
            rig.mass = Scale * mass;
        }
        else
        {
            if (isFrozen)
            {
                rig.drag = Scale * drag;
                rig.mass = Scale * mass;
            }
            else
            {
                rig.drag = drag;
                rig.mass = mass;
            }
        }
    }

    protected IEnumerator frozen()  //冰冻
    {
        float frozenTime = 2;
        float originScale = Scale;
        if (isFrozen)  //是否已经被冰冻
        {
            yield break;
        }
        isFrozen = true;
        GameObject iceFrag = Resources.Load<GameObject>("iceFrag");  //加载粒子效果  
        //---添加图层
        GameObject t = new GameObject();
        SpriteRenderer t_SR = t.AddComponent<SpriteRenderer>();
        if (SR.bounds.extents.x > SR.bounds.extents.y)  //选择图片
        {
            t_SR.sprite = Resources.LoadAll<Sprite>("iceCube")[1];
        }
        else
        {
            t_SR.sprite = Resources.LoadAll<Sprite>("iceCube")[0];
        }

        Bounds t_bounds = SR.bounds, bounds = t_SR.bounds;
        t.transform.position = t_bounds.center + new Vector3(0, 0, -0.01f);
        Vector2 scale = Vector2.zero;
        if (t_bounds.extents.x > t_bounds.extents.y)  //宽大于高
        {
            float _scale = t_bounds.extents.x / (bounds.extents.x * 0.41f);
            scale.x = _scale;
            scale.y = _scale;
        }
        else   //高大于宽
        {
            float _scale = t_bounds.extents.x / (bounds.extents.x * 0.41f);
            scale.x = _scale;
            scale.y = _scale;
        }
        t.transform.localScale = scale;
        t.transform.parent = transform;
        //----
        rig.velocity = Vector2.zero;
        Scale = 0.5f * originScale;  //改变摩擦系数

        float _time1 = 0;
        while (_time1 < frozenTime)  //被攻击提前碎掉
        {
            _time1 += Time.deltaTime;

            if (isFrozen == false)
            {
                Scale = originScale;
                isFrozen = false;
                GameObject _t_iceFrag = Instantiate(iceFrag, position: t.transform.position + new Vector3(0, 2, 0), rotation: Quaternion.Euler(0, 0, 0)) as GameObject;
                _t_iceFrag.transform.parent = transform;
                _t_iceFrag.transform.localScale = Vector3.one;
                Destroy(t);
                yield return new WaitForSeconds(_t_iceFrag.GetComponent<ParticleSystem>().duration);
                Destroy(_t_iceFrag);
                yield break;
            }
            yield return null;
        }

        Scale = originScale;
        isFrozen = false;
        GameObject t_iceFrag = Instantiate(iceFrag, position: t.transform.position + new Vector3(0, 2, 0), rotation: Quaternion.Euler(0, 0, 0)) as GameObject;
        t_iceFrag.transform.parent = transform;
        t_iceFrag.transform.localScale = Vector3.one;
        Destroy(t);
        yield return new WaitForSeconds(t_iceFrag.GetComponent<ParticleSystem>().duration);
        Destroy(t_iceFrag);
    }

    public void getHurt(int damage, Attribute attribute, int gameobjectID)
    {
        if (gameobjectID == gameObject.GetInstanceID())
        {
            if (isFrozen)
            {
                isFrozen = false;
                return;
            }
            if (attribute == Attribute.ice)
            {
                StartCoroutine(frozen());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.transform.tag;
        if (collision.transform.tag == "arms_player")  //是否为武器
        {
            int t_dir = CharacterControl.instance.transform.position.x >= SR.bounds.center.x ? -1 : 1;
            rig.AddForce(new Vector2(reactionForce * t_dir, 0));
        }

    }


}
