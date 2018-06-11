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
}
