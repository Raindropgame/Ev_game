using UnityEngine;
using System.Collections;

public class Platform_ablation : MonoBehaviour {

    //消融平台

    public float duration;  //可以站在平台上的时间
    public float ablationTime;  //消融的时间
    public float DisappearTime;  //消失的时间

    private float _time1 = 0,_time2 = 0;
    private BoxCollider2D BoxCol;
    private bool t = false,isRecovery = false;
    private float BurnScale_time = 0;
    private SpriteRenderer SR;

    private void Start()
    {
        BoxCol = GetComponent<BoxCollider2D>();
        SR = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(t)
        {          
            _time1 += Time.deltaTime;
            if(_time1 > duration)  
            {
                if(BoxCol.enabled == true)  //取消碰撞体
                {
                    BoxCol.enabled = false;
                }
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                SR.GetPropertyBlock(block);
                if(block.GetFloat("_BurnScale") >= 1)  
                {
                    _time2 += Time.deltaTime;
                    if(_time2 > DisappearTime)  //开始恢复
                    {
                        isRecovery = true;
                        t = false;
                        _time1 = 0;
                        _time2 = 0;
                        BurnScale_time = 0;
                    }
                }
                else   //开始消融
                {
                    BurnScale_time += Time.deltaTime;
                    MaterialPropertyBlock t_block = new MaterialPropertyBlock();
                    t_block.SetTexture("_MainTex", SR.sprite.texture);
                    t_block.SetFloat("_BurnScale", Mathf.Lerp(0, 1, BurnScale_time / ablationTime));
                    SR.SetPropertyBlock(t_block);
                }
            }
        }
        if(isRecovery)  //恢复
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            SR.GetPropertyBlock(block);
            if (block.GetFloat("_BurnScale") > 0)
            {
                BurnScale_time += Time.deltaTime;
                MaterialPropertyBlock t_block = new MaterialPropertyBlock();
                t_block.SetTexture("_MainTex", SR.sprite.texture);
                t_block.SetFloat("_BurnScale", Mathf.Lerp(1, 0, BurnScale_time / ablationTime));
                SR.SetPropertyBlock(t_block);
            }
            else
            {
                BoxCol.enabled = true;
                isRecovery = false;
                BurnScale_time = 0;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player" || collision.transform.tag == "enemy")
        {
            if(collision.contacts[0].point.y > this.transform.position.y)  //判断是否从平台上面发生碰撞 
            t = true;
            _time1 = 0;
            BurnScale_time = 0;
        }
    }


}
