using UnityEngine;
using System.Collections;

public class CharacterAttribute{

    private static CharacterAttribute instance;
    public static CharacterAttribute GetInstance()
    {
        if(instance == null)
        {
            instance = new CharacterAttribute();
        }
        return instance;
    }
    public int MaxBreath = 50;

    public int HP = 3;  // 生命值
    public float Breath = 50;  //气息值

    public float Speed_recover = 5;

    //气息消耗列表
    public int expend_run = 2;
    public int expend_attack = 5;
    public int expend_jump = 5;
    public int expend_dash = 10;
    public int expend_shoot = 3;
    public int expend_jumpshoot = 6;

}
