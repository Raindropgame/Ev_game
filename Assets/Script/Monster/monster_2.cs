using UnityEngine;
using System.Collections;

public class monster_2 : Monster_base {

	//魔花
    //看见玩家就攻击
    //玩家碰到会受伤
    //玩家靠近会后退
    //未看见玩家 徘徊

    public enum monster_2_state
    {
        walk,
        idle,
        attack
    }

    public monster_2_state currentState = monster_2_state.idle;
    public dir Dir = dir.left;

    private void FixedUpdate()
    {
        switch(currentState)
        {
            case monster_2_state.idle:

                break;
            case monster_2_state.attack:
                break;
            case monster_2_state.walk:
                break;
        }
    }

    void attack()  //攻击
    {

    }

}
