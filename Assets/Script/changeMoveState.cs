using UnityEngine;
using System.Collections;

public class changeMoveState : MonoBehaviour {


    //改变摄像机移动主角的方式

    public bool isIgnoreInit = true;  //是否忽略初始化时该脚本对相机移动方式的改变

    private bool isCharacterStay = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.name == "MainCamera" && (!Scene.instance.isInit || isIgnoreInit == false) && isCharacterStay)
        {
            switch (transform.tag)
            {
                case "onlyX":
                    CameraFollow.instance.changeMoveState(CameraMoveState.onlyX);
                    break;
                case "onlyY":
                    CameraFollow.instance.changeMoveState(CameraMoveState.onlyY);
                    break;
                default:
                    CameraFollow.instance.changeMoveState(CameraMoveState.both);
                    break;
            }
        }
        if (collision.transform.name == "character" && (!Scene.instance.isInit || isIgnoreInit == false))
        {
            isCharacterStay = true;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.name == "character")
        {
            isCharacterStay = false;
            CameraFollow.instance.changeMoveState(CameraMoveState.both);
        }
    }
}
