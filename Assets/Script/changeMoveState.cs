using UnityEngine;
using System.Collections;

public class changeMoveState : MonoBehaviour {


    //改变摄像机移动主角的方式

    //public bool isIgnoreInit = true;  //是否忽略初始化时该脚本对相机移动方式的改变

    public bool isCharacterStay = false;
    public bool isCameraStay = false;

    private BoxCollider2D Collider;

    private void Awake()
    {
        Collider = this.GetComponent<BoxCollider2D>();
    }

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
                    correctCameraPosition("onlyX", CameraFollow.instance.transform.position);
                    break;
                case "onlyY":
                    CameraFollow.instance.changeMoveState(CameraMoveState.onlyY);
                    correctCameraPosition("onlyY", CameraFollow.instance.transform.position);
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

    void correctCameraPosition(string state,Vector2 colliderPoint)  //修正摄像机的位置
    {
        switch(state)
        {
            case "onlyX":
                float center_y = Collider.offset.y + transform.position.y;
                float size_y = Collider.size.y * transform.lossyScale.y / 2.0f;
                if(colliderPoint.y > center_y)  //在碰撞体上方
                {
                    CameraFollow.instance.transform.position = new Vector3(CameraFollow.instance.transform.position.x, center_y + size_y, CameraFollow.instance.transform.position.z);
                }
                else  //下方
                {
                    CameraFollow.instance.transform.position = new Vector3(CameraFollow.instance.transform.position.x, center_y - size_y, CameraFollow.instance.transform.position.z);
                }
                break;
            case "onlyY":
                float center_x = Collider.offset.x + transform.position.x;
                float size_x = Collider.size.x * transform.lossyScale.x / 2.0f;
                if (colliderPoint.x > center_x)  //在碰撞体右方
                {
                    CameraFollow.instance.transform.position = new Vector3(center_x + size_x, CameraFollow.instance.transform.position.y, CameraFollow.instance.transform.position.z);
                }
                else  //左方
                {
                    CameraFollow.instance.transform.position = new Vector3(center_x - size_x, CameraFollow.instance.transform.position.y, CameraFollow.instance.transform.position.z);
                }
                break;
        }
    }
}
