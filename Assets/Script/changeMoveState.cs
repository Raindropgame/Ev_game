using UnityEngine;
using System.Collections;

public class changeMoveState : MonoBehaviour {


    //改变摄像机移动主角的方式

    //public bool isIgnoreInit = true;  //是否忽略初始化时该脚本对相机移动方式的改变

    private bool isCharacterStay = false;
    private bool isCameraStay = false;

    private BoxCollider2D Collider;

    private void Awake()
    {
        Collider = this.GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
        float t = 0;
        if(isCameraStay && isCharacterStay)  
        {
            switch (transform.tag)
            {
                case "onlyX":
                    t = getCameraAxis("onlyX", CameraFollow.instance.transform.position); 
                    CameraFollow.instance.changeMoveState(CameraMoveState.onlyX,t);
                    break;
                case "onlyY":
                    t = getCameraAxis("onlyY", CameraFollow.instance.transform.position);
                    CameraFollow.instance.changeMoveState(CameraMoveState.onlyY,t);
                    break;
                default:
                    CameraFollow.instance.changeMoveState(CameraMoveState.both,t);
                    break;
            }
        }
        if(isCameraStay && !isCharacterStay)
        {
            CameraFollow.instance.changeMoveState(CameraMoveState.both,t);
        }
        if(!isCameraStay && isCharacterStay)
        {
            CameraFollow.instance.changeMoveState(CameraMoveState.both,t);
        }
        if(!isCameraStay && !isCharacterStay)
        {
            CameraFollow.instance.changeMoveState(CameraMoveState.both,t);
        }
    }

    float getCameraAxis(string state,Vector2 colliderPoint)  //修正摄像机的位置
    {
        switch(state)
        {
            case "onlyX":
                float center_y = Collider.offset.y + transform.position.y;
                float size_y = Collider.size.y * transform.lossyScale.y / 2.0f;
                if(colliderPoint.y > center_y)  //在碰撞体上方
                {
                    return center_y + size_y;
                }
                else  //下方
                {
                    return center_y - size_y;
                }
            case "onlyY":
                float center_x = Collider.offset.x + transform.position.x;
                float size_x = Collider.size.x * transform.lossyScale.x / 2.0f;
                if (colliderPoint.x > center_x)  //在碰撞体右方
                {
                    return center_x + size_x;
                }
                else  //左方
                {
                    return center_x - size_x;
                }
        }

        return 0;
    }
}
