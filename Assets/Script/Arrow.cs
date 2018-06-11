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

    private void Awake()
    {
        particle = GetComponentInChildren<ParticleSystem>().emission;
        rig = this.GetComponent<Rigidbody2D>();
        SRenderer = this.GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        rig.velocity = new Vector2(CharacterControl.instance.Dir == dir.left ? -speed : speed, 0);
        SRenderer.flipX = CharacterControl.instance.Dir == dir.left ? true : false;

        boxCol.enabled = true;
        trailRenderer.enabled = true;
        particle.enabled = true;

        Vector3 characterPosition = CharacterControl.instance.transform.position;
        transform.parent = null; // 防止物体跟随主角
        transform.localPosition = CharacterControl.instance.Dir == dir.left ? new Vector3(characterPosition.x - 1.3f, characterPosition.y + 1.37f, characterPosition.z - 9) : new Vector3(characterPosition.x + 1.3f, characterPosition.y + 1.32f, -9);  //初始化位置

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "maps")
        {
            Invoke("TriggerWithEnemy", 0.03f);
        }
        if(collision.transform.tag == "enemy")
        {
            Invoke("TriggerWithEnemy",0.03f);
            CharacterObjectManager.instance.sendHurt(CharacterAttribute.GetInstance().jumpArrowAttack, Attribute.normal, collision.gameObject.GetInstanceID());
        }
    }


    void TriggerWithEnemy()
    {
        CharacterObjectManager.instance.recoveryArrow(this.gameObject);
        Instantiate(CharacterObjectManager.instance.arrow_end, position: transform.position, rotation: new Quaternion(0, 0, 0, 0));
        this.gameObject.SetActive(false);
    }

}