using UnityEngine;
using System.Collections;

public class add_soul : MonoBehaviour {

    //加灵魂值测试脚本

    public int add_num = 1;
    private float time = 2;
    private float _time = 0;
    bool t = false;
    private SpriteRenderer sRenderer;

    private void Start()
    {
        sRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (t)
        {
            _time += Time.deltaTime;
            if (_time > time)
            {
                sRenderer.color = new Color(sRenderer.color.r, sRenderer.color.g, sRenderer.color.b, 1f);
                _time = 0;
                t = false;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (t == false)
        {
            CharacterAttribute.GetInstance().add_HP(add_num);
            sRenderer.color = new Color(sRenderer.color.r, sRenderer.color.g, sRenderer.color.b, 0.5f);
            t = true;
        }
    }
}
