using UnityEngine;
using System.Collections;

public enum state  //人物状态
{
    normal,
    walk,
    run,
    dash,
    attack1,
    attack2,
    shoot,
    jump,
    jumpshoot,
    hurt,
    fall,
    endDash,
    Throw
};

public enum dir  //方向
{
    left,
    right
};

public enum CameraMoveState   //相机移动状态
{
    onlyX,
    onlyY,
    both
}

public enum DayOrNight  //昼夜
{
    Day,
    Night
} 

public enum weather  //天气
{
    Sunny,
    Rain,
    Thunder,
    RainAndThunder
}

public enum Attribute  //属性
{
    normal,  //中性
    lightning,  //电
    fire,   //火
    ice,    //冰
    wood    //木
}

public enum Arms  //武器
{
    swords,
    arrow,
    spear
}

public enum AbnormalState  //异常状态
{
    frozen,  //冰冻
}