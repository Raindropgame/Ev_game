using UnityEngine;
using System.Collections;

public class Monster_base : MonoBehaviour {

    //怪物的基类

    public Color hurtColor;
    public int MaxHP;
    [HideInInspector]
    public int currentHP;

    protected Texture texture;
    protected SpriteRenderer SR;

    private void Start()  //初始化
    {
        SR = GetComponent<SpriteRenderer>();
        texture = SR.material.GetTexture("_MainTex"); //获得纹理
        currentHP = MaxHP;
        CharacterObjectManager._sendHurt += getHurt;  //受伤消息(来自玩家)注册

        onStart();
    }

    private void OnDestroy()
    {
        CharacterObjectManager._sendHurt -= getHurt;  //解除绑定
    }

    public void getHurt(int damage, Attribute attribute,int gameobjectID)
    {
        if(gameobjectID == this.gameObject.GetInstanceID())  //是否是被攻击的对象
        {
            switch(attribute)   //根据受伤属性改变被攻击的颜色
            {
                case Attribute.normal:
                    hurtColor = Color.white;
                    break;
                case Attribute.fire:
                    hurtColor = GameData.getInstance().fireColor;
                    break;
                case Attribute.ice:
                    hurtColor = GameData.getInstance().iceColor;
                    break;
                case Attribute.wood:
                    hurtColor = GameData.getInstance().woodColor;
                    break;
                case Attribute.lightning:
                    hurtColor = GameData.getInstance().lightningColor;
                    break;
                default:
                    hurtColor = Color.white;
                    break;
            }
            _getHurt(damage,attribute);
        }
    }

    public IEnumerator beHurt()  //受伤反馈
    {
        MaterialPropertyBlock matBlock = new MaterialPropertyBlock();
        matBlock.SetColor("_hitColor", hurtColor);
        matBlock.SetFloat("_isHit", 1);
        matBlock.SetTexture("_MainTex", texture);
        SR.SetPropertyBlock(matBlock);
        yield return new WaitForSeconds(0.1f);  //变色的时间
        matBlock.SetFloat("_isHit", 0);
        SR.SetPropertyBlock(matBlock);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "lighting")
        {
            _getHurt(GameData.getInstance().lightningDamage, Attribute.lightning);
        }

        TriggerEnter();
    }

    virtual public void TriggerEnter()   
    {

    }

    virtual public void onStart()  //派生类中调用的start虚函数
    {

    }

    virtual public void _getHurt(int damage, Attribute attribute)  //受伤虚函数
    {

    }
}
