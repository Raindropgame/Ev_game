using UnityEngine;
using System.Collections;

public class Arrow_2 : MonoBehaviour {


    public float speed = 20;

    private Rigidbody2D rig;
    private BoxCollider2D boxCol;
    private TrailRenderer trailRenderer;
    private ParticleSystem.EmissionModule particle;
    private float tan;

    private void Awake()
    {
        tan = Mathf.Tan(transform.localEulerAngles.z * Mathf.Deg2Rad);
        rig = this.GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        particle = GetComponentInChildren<ParticleSystem>().emission;
    }

    private void OnEnable()
    {
        rig.velocity = new Vector2(CharacterControl.instance.Dir == dir.left ? -1 : 1, tan) * speed;  //初始速度

        boxCol.enabled = true;
        trailRenderer.enabled = true;
        particle.enabled = true;

        transform.parent = null; // 防止物体跟随主角

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "maps")
        {
            Invoke("TriggerWithEnemy", 0.03f);
        }
        if(collision.transform.tag == "enemy")
        {
            Invoke("TriggerWithEnemy", 0.03f);
            CharacterObjectManager.instance.sendHurt(CharacterAttribute.GetInstance().jumpArrowAttack, Attribute.normal, collision.gameObject.GetInstanceID());
        }
    }


    void TriggerWithEnemy()  //碰到敌人
    {
        CharacterObjectManager.instance.recoveryArrow_2(this.gameObject);
        Instantiate(CharacterObjectManager.instance.arrow_end, position: transform.position, rotation: new Quaternion(0, 0, 0, 0));
    }
}
