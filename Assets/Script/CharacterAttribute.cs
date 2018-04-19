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

    public int HP = 3;  // 生命值
    public int Breath = 50;  //气息值
    


}
