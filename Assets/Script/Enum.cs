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
    endDash
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