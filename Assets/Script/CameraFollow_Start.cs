using UnityEngine;
using System.Collections;

public class CameraFollow_Start : MonoBehaviour {
    private GameObject chara;
    private float speed;

    private void Start()
    {
        chara = GameObject.Find("character");
        speed = ((Vector2)(chara.transform.position - transform.position)).magnitude / 0.6f;
    }

    // Update is called once per frame
    void FixedUpdate () {

        switch (CameraFollow.instance.moveState)
        {
            case CameraMoveState.both:
                transform.position = Vector2.MoveTowards(transform.position, chara.transform.position, speed * Time.deltaTime);
                break;
            case CameraMoveState.onlyY:
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, chara.transform.position.y), speed * Time.deltaTime);
                break;
            case CameraMoveState.onlyX:
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(chara.transform.position.x, transform.position.y), speed * Time.deltaTime);
                break;
        }
    }
}
