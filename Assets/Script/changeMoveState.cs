using UnityEngine;
using System.Collections;

public class changeMoveState : MonoBehaviour {


    //改变摄像机移动主角的方式

    //public bool isIgnoreInit = true;  //是否忽略初始化时该脚本对相机移动方式的改变

    public bool isCharacterStay = false;
    public bool isCameraStay = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*if (collision.transform.name == "MainCamera" && (!Scene.instance.isInit || isIgnoreInit == false) && isCharacterStay)
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
        }*/
        if(collision.tag == "Player")
        {
            isCharacterStay = true;
            changeState();
        }
        if(collision.tag == "MainCamera")
        {
            isCameraStay = true;
            changeState();
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        /*if (collision.transform.name == "character")
        {
            isCharacterStay = false;
            CameraFollow.instance.changeMoveState(CameraMoveState.both);
        }*/
        if (collision.tag == "Player")
        {
            isCharacterStay = false;
            changeState();
        }
        if (collision.tag == "MainCamera")
        {
            isCameraStay = false;
            changeState();
        }
    }

    void changeState()//改变摄像机跟随状态
    {
        if(isCameraStay && isCharacterStay)  
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
        if(isCameraStay && !isCharacterStay)
        {
            CameraFollow.instance.changeMoveState(CameraMoveState.both);
        }
        if(!isCameraStay && isCharacterStay)
        {
            CameraFollow.instance.changeMoveState(CameraMoveState.both);
        }
        if(!isCameraStay && !isCharacterStay)
        {
            CameraFollow.instance.changeMoveState(CameraMoveState.both);
        }
    }
}
