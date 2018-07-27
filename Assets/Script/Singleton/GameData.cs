using UnityEngine;
using System.Collections;

public class GameData{

    //管理游戏数据

    static public int lightningDamage = 2;  //雷电伤害量

    //各个属性的代表颜色
    static public Color normalColor = Color.white;
    static public Color fireColor = new Color(1, 0.39f, 0.12f);
    static public Color iceColor = new Color(0.38f, 0.95f, 1f);
    static public Color woodColor = new Color(0.71f, 0.56f, 0.1f);
    static public Color lightningColor = new Color(1f, 1f, 0.46f);

    //异常状态各种数据
    static public float gravityScale_stone = 5;
    static public float dragScale_stone = 2;
    static public float frozenTime = 2;
    static public float stoneTime = 5;
    static public float burningTime = 5;
    static public float burningSpaceTime = 2;
    static public int burningDamage = 1;
    static public float burning_proba = 1;


    static public float fire_boom_stopTime = 0.12f;  //火焰爆炸的卡顿时间
    static public float die_stopTime = 0.1f;  //死亡时卡顿时间

    //震屏的默认数据
    static public float defualt_shakeScale = 0.25f;
    static public float defualt_singleTime = 0.04f;
    static public float defualt_shakeTime = 0.2f;

    static public Color Attribute2Color(Attribute t)
    {
        Color o = normalColor;
        switch(t)
        {
            case Attribute.fire:
                o = fireColor;
                break;
            case Attribute.ice:
                o = iceColor;
                break;
            case Attribute.lightning:
                o = lightningColor;
                break;
            case Attribute.normal:
                o = normalColor;
                break;
            case Attribute.wood:
                o = woodColor;
                break;
        }
        return o;
    }
}
