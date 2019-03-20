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

    CharacterAttribute()
    {
        Init();  //读取存档
    }

    public int MaxBreath = 50;  //最大气息值
    private float m_Breath = 50;  //当前气息值
    public float Breath
    {
        get
        {
            if(isOverLoad_breath)
            {
                return 0;
            }
            else
            {
                return m_Breath;
            }
        }
        set
        {
            if(value < 0)
            {
                m_Breath = 0;
            }
            else if(value > MaxBreath)
            {
                m_Breath = MaxBreath;
            }
            m_Breath = value;
        }
    }
    public float Breath_real
    {
        get
        {
            return m_Breath;
        }
    }
    public bool isOverLoad_breath = false;  //是否过载

    public int MaxHP = 3;   //最大生命值
    public int HP = 3;  // 当前生命值


    //气息消耗列表
    public int expend_run = 5;
    public int expend_attack = 8;
    public int expend_jump = 20;
    public int expend_dash = 10;
    public int expend_shoot = 10;
    public int expend_jumpshoot = 15;
    public int expend_throw = 10;

    //各个武器攻击力
    public int[] Attack = new int[3] { 1,1,1};

    //武器属性
    public Attribute[] ArmsAttribute = new Attribute[3] { Attribute.normal, Attribute.normal, Attribute.normal };

    public int Speed_recovery = 30;  //气息恢复速度
    public int Speed_recovery_overload = 25;  //过载气息恢复速度

    //能力
    public bool[] isEnable = { true, false, true, false, true, true, false, true, false, true, true, true,true };

    public int JumpTimes = 1;    //跳跃次数
    public int MaxJumpShootTimes = 1;  //跳射次数

    //武器槽
    public ArmsGemGroove[] ArmsGemGroove = new ArmsGemGroove[3] { new global::ArmsGemGroove(), new global::ArmsGemGroove(), new global::ArmsGemGroove() };

    public void Init()  //读取存档
    {
        MaxBreath = PlayerPrefs.GetInt("MaxBreath", MaxBreath);
        MaxHP = PlayerPrefs.GetInt("MaxHP", MaxHP);
        HP = MaxHP;
        expend_run = PlayerPrefs.GetInt("expend_run", expend_run);
        expend_attack = PlayerPrefs.GetInt("expend_attack", expend_attack);
        expend_jump = PlayerPrefs.GetInt("expend_jump", expend_jump);
        expend_dash = PlayerPrefs.GetInt("expend_dash", expend_dash);
        expend_shoot = PlayerPrefs.GetInt("expend_shoot", expend_shoot);
        expend_jumpshoot = PlayerPrefs.GetInt("expend_jumpshoot", expend_jumpshoot);
        PlayerPrefs.GetInt("expend_throw", expend_throw);
        Speed_recovery = PlayerPrefs.GetInt("Speed_recovery", Speed_recovery);
        JumpTimes = PlayerPrefs.GetInt("JumpTimes", JumpTimes);
        MaxJumpShootTimes = PlayerPrefs.GetInt("MaxJumpShootTimes", MaxJumpShootTimes);
        for (int i = 0; i < isEnable.Length; i++)
        {
            isEnable[i] = PlayerPrefs.GetInt(((state)i).ToString(), isEnable[i] == true ? 1 : 0) == 1 ? true : false;  //true:1  false:0
        }
    }

    public void SetFile()  //存档
    {
        PlayerPrefs.SetInt("MaxBreath", MaxBreath);
        PlayerPrefs.SetInt("MaxHP", MaxHP);
        PlayerPrefs.SetInt("expend_run", expend_run);
        PlayerPrefs.SetInt("expend_attack", expend_attack);
        PlayerPrefs.SetInt("expend_jump", expend_jump);
        PlayerPrefs.SetInt("expend_dash", expend_dash);
        PlayerPrefs.SetInt("expend_shoot", expend_shoot);
        PlayerPrefs.SetInt("expend_jumpshoot", expend_jumpshoot);
        PlayerPrefs.SetInt("expend_throw", expend_throw);
        PlayerPrefs.SetInt("Speed_recovery", Speed_recovery);
        PlayerPrefs.SetInt("JumpTimes", JumpTimes);
        PlayerPrefs.SetInt("MaxJumpShootTimes", MaxJumpShootTimes);
        for (int i = 0; i < isEnable.Length; i++)
        {
            PlayerPrefs.SetInt(((state)i).ToString(), isEnable[i] == true ? 1 : 0);  //true:1  false:0
        }
    }

    public int reduce_HP(int damage)  //减血  并返回减少后的血量值
    {
        if(damage > HP)
        {
            HP = 0;
        }
        else
        {
            HP -= damage;
        }
        return HP;
    }

    public int add_HP(int num)  //回复体力  并返回当前血量值
    {
        if(HP + num >= MaxHP)
        {
            HP = MaxHP;
        }
        else
        {
            HP += num;
        }
        CharacterObjectManager.instance.PLayTreat();
        return HP;
    }

    public void add_MaxHP(int num)  //增加最大血量值
    {
        MaxHP += num;
    }

    //金币
    private int m_coinNum = 0;
    public int coinNum
    {
        get
        {
            return m_coinNum;
        }
        set
        {
            if (value < 0)
            {
                m_coinNum = 0;
            }
            else
            {
                m_coinNum = value;
            }
        }
    }
}
