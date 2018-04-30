using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public float lastTime = 0.5f;
    public float smoothTime = 1;
    public float t = 0.5f;
    public static CameraFollow instance;
    public bool CameraIsMove;

    private GameObject character;
    private float tTime = 0;
    private bool isMoving = false;
    Vector3 targetPosition;
    private Rigidbody2D charaRig;
    private Vector3 currentV;
    [HideInInspector]
    public CameraMoveState moveState = CameraMoveState.both;


    private void Awake()
    {
        instance = this;
        character = GameObject.Find("character");
        charaRig = character.GetComponent<Rigidbody2D>();
        targetPosition = character.transform.position;
        targetPosition.z = this.transform.position.z;
    }

    private void Update()
    {
        CameraIsMove = false;
        Vector3 temp = this.transform.position - character.transform.position;
        if (Mathf.Abs(temp.x) > t || Mathf.Abs(temp.y) > t) // 判断摄像机是否在角色附近
        {
            CameraIsMove = true;
            if ((Vector2)character.transform.position != (Vector2)targetPosition)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
            tTime += Time.deltaTime;
            if (tTime >= lastTime)
            {
                if (isMoving)
                {
                    switch(moveState)
                    {
                        case CameraMoveState.both:
                            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, charaRig.velocity.magnitude * Time.deltaTime);  // 如果人物在移动则摄像机速度=人物速度
                            break;
                        case CameraMoveState.onlyY:
                            this.transform.position = new Vector3(this.transform.position.x, Vector2.MoveTowards(this.transform.position, targetPosition, charaRig.velocity.magnitude * Time.deltaTime).y, this.transform.position.z);
                            break;
                        case CameraMoveState.onlyX:
                            this.transform.position = new Vector3(Vector2.MoveTowards(this.transform.position, targetPosition, charaRig.velocity.magnitude * Time.deltaTime).x, this.transform.position.y, this.transform.position.z);
                            break;
                    }
                }
                else
                {
                    /*tTime2 += Time.deltaTime;
                    float t = tTime2 / arriveTime;
                    this.transform.position = new Vector3(Mathf.SmoothStep(this.transform.position.x, targetPosition.x, t),this.transform.position.y,targetPosition.z);
                    this.transform.position = new Vector3(this.transform.position.x, Mathf.SmoothStep(this.transform.position.y,targetPosition.y,t), targetPosition.z);*/
                    switch (moveState)
                    {
                        case CameraMoveState.both:
                            this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPosition, ref currentV, smoothTime);
                            break;
                        case CameraMoveState.onlyY:
                            this.transform.position = new Vector3(this.transform.position.x, Mathf.SmoothDamp(this.transform.position.y, targetPosition.y, ref currentV.y, smoothTime), this.transform.position.z);
                            break;
                        case CameraMoveState.onlyX:
                            this.transform.position = new Vector3(Mathf.SmoothDamp(this.transform.position.x, targetPosition.x, ref currentV.x, smoothTime), this.transform.position.y, this.transform.position.z);
                            break;
                    }
                }
            }
        }
        else
        {
            tTime = 0;
        }
        targetPosition = character.transform.position;
        targetPosition.z = this.transform.position.z;
    }

    public void changeMoveState(CameraMoveState t)
    {
        moveState = t;
    }
}
