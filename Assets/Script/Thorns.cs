using UnityEngine;
using System.Collections;

public class Thorns : MonoBehaviour {

    [Header("伤害")]
    public int damage;
    [Header("动画属性")]
    public float speed;
    public float stretchScale;
    public float waveScale;
    public Vector2 pivot;
    public float cycle;
    public Color color_1, color_2;

    private SpriteRenderer SR;
    private Vector3 t_vec3_0;
    private float origin_y;
    private float random_factor;
    private BoxCollider2D Coll;

	void Start () {
        Coll = GetComponent<BoxCollider2D>();
        SR = GetComponent<SpriteRenderer>();
        MaterialPropertyBlock MB = null;
        MB = new MaterialPropertyBlock();
        origin_y = transform.localScale.y;
        //Random.InitState(transform.GetInstanceID());
        random_factor = Random.Range(0.0f,cycle);

        MB.SetFloat("_Speed", speed);
        MB.SetFloat("_Scale", waveScale);
        MB.SetFloat("_Pivot_x", pivot.x);
        MB.SetFloat("_Pivot_y", pivot.y);
        MB.SetTexture("_MainTex", SR.sprite.texture);

        SR.SetPropertyBlock(MB);
    }

    private void Update()
    {
        t_vec3_0 = transform.localScale;
        t_vec3_0.y =  (Mathf.Sin(Time.time * (1 / cycle) + random_factor)) * (stretchScale - 1 ) + origin_y;
        SR.color = Color.Lerp(color_1, color_2, (0.5f * Mathf.Sin(Time.time * (1 / cycle) + random_factor) + 0.5f));
        transform.localScale = t_vec3_0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.CompareTo("Player") == 0)
        {
            CharacterControl.instance.hurt(damage, Attribute.normal, Coll.bounds.center);

            return;
        }
        if(collision.tag.CompareTo("arms_player") == 0)
        {
            shake(0.08f);
        }
    }

    private bool isShake = false;
    void shake(float shakeScale)
    {
        if(!isShake)
        {
            StartCoroutine(IE_shake(shakeScale));
        }
    }

    IEnumerator IE_shake(float shakeScale)
    {
        const float shake_cycle = 0.03f;
        const float shake_duuration = 0.2f;
        Vector3 origin_pos = this.transform.position;
        float _time0 = 0, _time1 = 0;
        isShake = true;
        while(_time0 < shake_duuration)
        {
            _time0 += Time.deltaTime;
            _time1 += Time.deltaTime;

            if(_time1 > shake_cycle)
            {
                _time1 = 0;
                this.transform.position = origin_pos + (Vector3)(Random.insideUnitCircle * shakeScale);
            }
            yield return null;
        }
        isShake = false;
        this.transform.position = origin_pos;
    }

    //----优化
    private void OnBecameVisible()
    {
        this.enabled = true;
    }

    private void OnBecameInvisible()
    {
        this.enabled = false;
    }
    //-------
}
