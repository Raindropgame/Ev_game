using UnityEngine;
using System.Collections;

public class add_maxSoul : MonoBehaviour {

    //加最大灵魂值测试脚本

    public int add_num = 1;
    private float time = 2;
    private float _time = 0;
    bool t = false;
    private void FixedUpdate()
    {
        if(t)
        {
            _time += Time.deltaTime;
            if(_time > time)
            {
                _time = 0;
                t = false;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (t == false)
        {
            CharacterAttribute.GetInstance().add_MaxHP(add_num);
            t = true;
        }
    }
}
