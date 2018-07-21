using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{

    public float speed = 20;

    private Rigidbody2D rig;
    private SpriteRenderer SRenderer;
    private BoxCollider2D boxCol;
    private TrailRenderer trailRenderer;
    private ParticleSystem.EmissionModule particle;
    private SpriteRenderer SR;
    private Attribute currentAttribute = Attribute.normal;
    private int damage;

    private void Awake()
    {
        particle = GetComponentInChildren<ParticleSystem>().emission;
        rig = this.GetComponent<Rigidbody2D>();
        SRenderer = this.GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        SR = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.arrow].GemWork();   //结晶作用

        rig.velocity = new Vector2(CharacterControl.instance.Dir == dir.left ? -speed : speed, 0);
        SRenderer.flipX = CharacterControl.instance.Dir == dir.left ? true : false;

        boxCol.enabled = true;
        trailRenderer.enabled = true;
        particle.enabled = true;
        isTrigger = false;

        Vector3 characterPosition = CharacterControl.instance.transform.position;
        transform.parent = null; // 防止物体跟随主角
        transform.localPosition = CharacterControl.instance.Dir == dir.left ? new Vector3(characterPosition.x - 1.3f, characterPosition.y + 1.37f, characterPosition.z - 9) : new Vector3(characterPosition.x + 1.3f, characterPosition.y + 1.32f, -9);  //初始化位置

        //根据属性改变颜色
        Material a = trailRenderer.material;
        a.SetColor("_Color", GameData.getInstance().Attribute2Color(CharacterAttribute.GetInstance().ArmsAttribute[(int)Arms.arrow]));  //颜色减淡
        SR.color = GameData.getInstance().Attribute2Color(CharacterAttribute.GetInstance().ArmsAttribute[(int)Arms.arrow]);

        //获取当前属性和伤害
        currentAttribute = CharacterAttribute.GetInstance().ArmsAttribute[(int)Arms.arrow];
        damage = CharacterAttribute.GetInstance().Attack[(int)Arms.arrow];


    }

    private bool isTrigger = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTrigger)
        {
            if (collision.transform.tag == "maps")
            {
                isTrigger = true;
                Invoke("TriggerWithEnemy", 0.03f);
                CharacterObjectManager.instance.sendHurt(damage, currentAttribute, collision.gameObject.GetInstanceID());
            }
            if (collision.transform.tag == "enemy")
            {
                isTrigger = true;
                Invoke("TriggerWithEnemy", 0.03f);
                CharacterObjectManager.instance.sendHurt(damage, currentAttribute, collision.gameObject.GetInstanceID());
            }
        }
    }


    void TriggerWithEnemy()
    {
        CharacterObjectManager.instance.recoveryArrow(this.gameObject);
        Instantiate(CharacterObjectManager.instance.arrow_end, position: transform.position, rotation: Quaternion.Euler(0,0,0));
        this.gameObject.SetActive(false);
    } 

}