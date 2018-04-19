using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public float lastTime = 0.5f;
    public float smoothTime = 1;
    public float t = 0.5f;

    private GameObject character;
    private float tTime = 0;
    private bool isMoving = false;
    Vector3 targetPosition;
    private Rigidbody2D charaRig;
    private Vector3 currentV;


    private void Awake()
    {
        character = GameObject.Find("character");
        charaRig = character.GetComponent<Rigidbody2D>();
        targetPosition = character.transform.position;
        targetPosition.z = this.transform.position.z;
    }

    private void FixedUpdate()
    {
        Vector3 temp = this.transform.position - character.transform.position;
        if (Mathf.Abs(temp.x) > t || Mathf.Abs(temp.y) > t) // 判断摄像机是否在角色附近
        {
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
                    this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, charaRig.velocity.magnitude * Time.deltaTime);  // 如果人物在移动则摄像机速度=人物速度
                }
                else
                {
                    /*tTime2 += Time.deltaTime;
                    float t = tTime2 / arriveTime;
                    this.transform.position = new Vector3(Mathf.SmoothStep(this.transform.position.x, targetPosition.x, t),this.transform.position.y,targetPosition.z);
                    this.transform.position = new Vector3(this.transform.position.x, Mathf.SmoothStep(this.transform.position.y,targetPosition.y,t), targetPosition.z);*/
                    this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPosition, ref currentV, smoothTime);
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
}
