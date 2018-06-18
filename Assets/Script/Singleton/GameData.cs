using UnityEngine;
using System.Collections;

public class GameData{

    //管理游戏数据

    private static GameData instance;

    public static GameData getInstance()
    {
        if(instance == null)
        {
            instance = new GameData();
        }
        return instance;
    }

    public int lightningDamage = 2;  //雷电伤害量

    //各个属性的代表颜色
    public Color normalColor = Color.white;
    public Color fireColor = new Color(1, 0.39f, 0.12f);
    public Color iceColor = new Color(0.38f, 0.95f, 1f);
    public Color woodColor = new Color(0.71f, 0.56f, 0.1f);
    public Color lightningColor = new Color(1f, 1f, 0.46f);

    public Color Attribute2Color(Attribute t)
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
