using UnityEngine;
using System.Collections;

public class Arrow_2 : MonoBehaviour {

    public float speed = 20;

    private Rigidbody2D rig;
    private SpriteRenderer SRenderer;
    private BoxCollider2D boxCol;
    private TrailRenderer trailRenderer;
    private float tan;

    private void Awake()
    {
        tan = Mathf.Tan(transform.localEulerAngles.z * Mathf.Deg2Rad);
        rig = this.GetComponent<Rigidbody2D>();
        SRenderer = this.GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        rig.velocity = new Vector2(CharacterControl.instance.Dir == dir.left ? -1 : 1, tan) * speed;  //初始速度

        boxCol.enabled = true;
        trailRenderer.enabled = true;

        transform.parent = null; // 防止物体跟随主角
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "maps")
        {
            setFalse(collision);
        }
    }

    void setFalse(Collider2D collision)
    {
        rig.velocity = new Vector2(0, 0);   //碰到地形停止
        this.transform.parent = collision.transform;
        boxCol.enabled = false;
        trailRenderer.enabled = false;

    }
}
