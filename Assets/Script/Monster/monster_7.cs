using UnityEngine;
using System.Collections;

public class monster_7 : Monster_base {

	enum monster_7_state
    {
        walk,
        shrink,
        shrinkBack
    }

    [Header("自身属性")]
    public float walkSpeed;
    public float shrinkDuration;

    private monster_7_state currentState;
    private bool _isNearWall = false;
    private bool _isGround = false;
    private bool _isNearEdge = false;
    private RaycastHit2D hitPoint;
    private float Timer_shrink;

    public override void onStart()
    {
        base.onStart();

        Dir = dir.left;
        changeState(currentState);
    }

     void Update()
    {
        base._FixedUpdate();

        _isGround = isGround();
        _isNearWall = isNearWall();
        _isNearEdge = isNearEdge();
        switch(currentState)
        {
            case monster_7_state.walk:
                if (_isGround)
                {
                    rig.velocity = (Dir == dir.left ? Vector2.left : Vector2.right) * walkSpeed;

                    if(_isNearWall || _isNearEdge)
                    {
                        changeDir(Dir == dir.left ? dir.right : dir.left);
                    }
                }
                break;
            case monster_7_state.shrink:
                Timer_shrink += Time.deltaTime;

                if(Timer_shrink > shrinkDuration)
                {
                    changeState(monster_7_state.shrinkBack);
                }
                break;
            case monster_7_state.shrinkBack:
                
                if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
                {
                    changeState(monster_7_state.walk);
                }
                break;
        }
    }

    void changeState(monster_7_state t)
    {
        if (currentState != t)
        {
            animator.SetTrigger(t.ToString());
            currentState = t;
        }
    }

    bool isNearWall()
    {
        int mask = (1 << 9) | (1 << LayerMask.NameToLayer("Gravity"));
        if (Dir == dir.left)
        {
            GameFunction.t_Vector2 = (Vector2)gravity.bounds.min + (Vector2.left + Vector2.up) * 0.02f;
        }
        else
        {
            GameFunction.t_Vector2 = ((Vector2)gravity.bounds.min + gravity.bounds.size.x * Vector2.right) + Vector2.one * 0.02f;
        }

        hitPoint = Physics2D.Raycast(GameFunction.t_Vector2, Vector2.up, gravity.bounds.size.y, mask);
        if (hitPoint.transform != null)
        {
             return true;
        }
        return false;
    }

    bool isNearEdge()
    {
        int mask = 1 << 9;

        if (Dir == dir.left)
        {
            GameFunction.t_Vector2 = (Vector2)gravity.bounds.min + (Vector2.left + Vector2.up) * 0.01f;
        }
        else
        {
            GameFunction.t_Vector2 = ((Vector2)gravity.bounds.min + gravity.bounds.size.x * Vector2.right) + Vector2.one * 0.01f;
        }

        hitPoint = Physics2D.Raycast(GameFunction.t_Vector2, Vector2.down, 1f, mask);
        if (hitPoint.transform != null)
        {
            return false;
        }
        return true;
    }

    public override void _getHurt(int damage, Attribute attribute, Vector2 ColliderPos)
    {
        base._getHurt(damage, attribute, ColliderPos);

        changeState(monster_7_state.shrink);
        Timer_shrink = 0;
    }
}
