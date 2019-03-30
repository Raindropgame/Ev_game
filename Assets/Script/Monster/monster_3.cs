using UnityEngine;
using System.Collections;

public class monster_3 : Monster_base {

    //触手
    //凶恶
    //极强的攻击性

    public SpriteRenderer[] _SR;
    public GameObject boom_particle;

    private Texture[] _texture = new Texture[4];

    public override void onStart()
    {
        base.onStart();

        for(int i = 0;i<_SR.Length;i++)
        {
            _texture[i] = _SR[i].sprite.texture;
        }

    }

    private float lastTime = 0;
    override public void _getHurt(int damage, Attribute attibute, Vector2 ColliderPos)
    {
        if (lastTime + 0.05f < Time.time)  //防止多个碰撞体重复计算伤害
        {
            currentHP -= damage;
            if (currentHP <= 0)  //是否死亡
            {
                StartCoroutine(die());
                return;
            }
            StartCoroutine(beHurt());
        }
        lastTime = Time.time;
    }

    override protected IEnumerator die()  //死亡
    {
        yield return null;
        boom_particle.SetActive(true);
        boom_particle.transform.parent = null;
        Destroy(this.gameObject);
        CameraFollow.instance.shakeCamera(0.25f, 0.04f, 0.2f);  //镜头抖动
        CameraFollow.instance.Stutter(0.15f);
        
    }

    override public IEnumerator beHurt()  //受伤反馈
    {
        //被攻击时的闪烁
        MaterialPropertyBlock matBlock = new MaterialPropertyBlock();
        for (int i = 0; i < _SR.Length; i++)
        {
            matBlock.SetColor("_hitColor", hurtColor);
            matBlock.SetFloat("_isHit", 1);
            matBlock.SetTexture("_MainTex", _texture[i]);
            _SR[i].SetPropertyBlock(matBlock);
        }
        yield return new WaitForSeconds(0.1f);  //变色的时间
        for (int i = 0; i < _SR.Length; i++)
        {
            matBlock.SetFloat("_isHit", 0);
            _SR[i].SetPropertyBlock(matBlock);
        }
        //----------
    }

    private void OnBecameVisible()
    {
        this.enabled = true;
    }

    private void OnBecameInvisible()
    {
        this.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
    }


}
