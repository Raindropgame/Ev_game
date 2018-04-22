using UnityEngine;
using System.Collections;

public enum state
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

public enum dir
{
    left,
    right
};

public enum CameraMoveState
{
    onlyX,
    onlyY,
    both
}

