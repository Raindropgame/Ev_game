using UnityEngine;
using System.Collections;

public class MyInput : MonoBehaviour {

    static public MyInput instance;

    private float Horizontal = 0;
    private float lastHorizontal = 0;
    private KeyCode jump = KeyCode.JoystickButton0;
    private bool isJump = false;
    private bool isJumpDown = false;
    private KeyCode shoot = KeyCode.JoystickButton3;
    private bool isShoot = false;
    private KeyCode attack = KeyCode.JoystickButton2;
    private bool isAttack = false;
    private KeyCode dash = KeyCode.JoystickButton1;
    private bool isDash = false;
    private KeyCode bag = KeyCode.JoystickButton4;
    private bool isBag = false;
    private KeyCode Throw = KeyCode.JoystickButton5;
    private bool isThrow = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        isJump = false;
        isShoot = false;
        isAttack = false;
        isDash = false;
        isBag = false;
        isThrow = false;
        isJumpDown = false;

        lastHorizontal = Horizontal;
        Horizontal = Input.GetAxis("Horizontal");
        isJump = Input.GetKey(jump);
        isJumpDown = Input.GetKeyDown(jump);
        isShoot = Input.GetKeyDown(shoot);
        isAttack = Input.GetKeyDown(attack);
        isDash = Input.GetKeyDown(dash);
        isBag = Input.GetKeyDown(bag);
        isThrow = Input.GetKeyDown(Throw);

    }

    public bool isGetRight()
    {
        if (Horizontal > 0)
            return true;
        if (Input.GetKey(KeyCode.RightArrow))
            return true;
        return false;
    }

    public bool isGetLeft()
    {
        if (Horizontal < 0)
            return true;
        if (Input.GetKey(KeyCode.LeftArrow))
            return true;
        return false;
    }

    public bool isGetRightDown()
    {
        if (lastHorizontal <= 0 && Horizontal > 0)
            return true;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            return true;
        return false;
    }

    public bool isGetLeftDown()
    {
        if (lastHorizontal >= 0 && Horizontal < 0)
            return true;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            return true;
        return false;
    }

    public bool isGetRightUp()
    {
        if (lastHorizontal > 0 && Horizontal <= 0)
            return true;
        if (Input.GetKeyUp(KeyCode.RightArrow))
            return true;
        return false;
    }

    public bool isGetLeftUp()
    {
        if (lastHorizontal < 0 && Horizontal >= 0)
            return true;
        if (Input.GetKeyUp(KeyCode.LeftArrow))
            return true;
        return false;
    }

    public bool isGetJump()
    {
        if (Input.GetKey(KeyCode.Space))
            return true;
        return isJump;
    }

    public bool isGetJumpDown()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            return true;
        return isJumpDown;
    }

    public bool isGetAttack()
    {
        if (Input.GetKeyDown(KeyCode.X))
            return true;
        return isAttack;
    }

    public bool isGetShoot()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            return true;
        return isShoot;
    }

    public bool isGetDash()
    {
        if (Input.GetKeyDown(KeyCode.C))
            return true;
        return isDash;
    }

    public bool isGetBag()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            return true;
        return isBag;
    }

    public bool isGetThrow()
    {
        if (Input.GetKeyDown(KeyCode.S))
            return true;
        return isThrow;
    }

    public bool isGetRun()
    {
        if(Mathf.Abs(Horizontal) > 0.99f)
        {
            return true;
        }
        return false;
    }
}
