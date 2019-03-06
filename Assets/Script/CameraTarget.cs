using UnityEngine;
using System.Collections;

public class CameraTarget : MonoBehaviour {

    //控制相机目标位置

    private bool isCharacterStay = false;
    private bool isCameraStay = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isCharacterStay = true;
            changeState();
        }
        if (collision.tag == "MainCamera")
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

    void changeState()
    {
        if (isCameraStay && isCharacterStay)
        {
            CameraFollow.instance.StayAt(transform.position);
        }
        else if (isCameraStay && !isCharacterStay)
        {
            CameraFollow.instance.unStay();
        }
        else if (!isCameraStay && isCharacterStay)
        {
            CameraFollow.instance.StayAt(transform.position);
        }
        else if (!isCameraStay && !isCharacterStay)
        {
            CameraFollow.instance.unStay();
        }
    }
}
