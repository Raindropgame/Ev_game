using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{

    public float speed = 20;

    private Rigidbody2D rig;
    private SpriteRenderer SRenderer;
    private BoxCollider2D boxCol;
    private TrailRenderer trailRenderer;

    private void Awake()
    {
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

        Vector3 characterPosition = CharacterControl.instance.transform.position;
        transform.parent = null; // 防止物体跟随主角
        transform.localPosition = CharacterControl.instance.Dir == dir.left ? new Vector3(characterPosition.x - 1.3f, characterPosition.y + 1.37f, characterPosition.z + 9) : new Vector3(characterPosition.x + 1.3f, characterPosition.y + 1.32f, characterPosition.z + 9);  //初始化位置


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "map")
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