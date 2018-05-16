using UnityEngine;
using System.Collections;

public class Lighting : MonoBehaviour {

    //随机生成闪电

    public float BendScale;  //弯曲程度
    public int BendTimes;  //弯曲次数
    public float Deep;   //闪电的Z轴
    public float Speed;   //闪电播放速度
    public float overClose;  //播放完毕多少秒后关闭闪电

    private LineRenderer LineRender;
    private Vector3[] Point;
    private Vector3 startPos = Vector3.zero;  //起始点
    private Vector3 endPos;   //结束点
    private int currentPoint = 0;
    private bool isActive = false;
    private EdgeCollider2D ECollider;

    private void Awake()
    {
        ECollider = this.GetComponent<EdgeCollider2D>();
        Point = new Vector3[1 + (int)Mathf.Pow(2,BendTimes)];
        LineRender = this.GetComponent<LineRenderer>();
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        isActive = true;
        float X_offset = 10;
        startPos = Thunder.instance.getStartPos();
        endPos = new Vector2(Random.Range(startPos.x - X_offset, startPos.x + X_offset),Thunder.instance.CameraTrans.position.y + 0 - Random.Range(15, 30));
        GeneratePoint();
        currentPoint = 0;

        ECollider.enabled = true;
        changeColliderPoint();  //更新碰撞体
    }

    private void Update()
    {
        if (isActive)
        {
            int num = (int)(Time.deltaTime / Speed);
            for (int i = 0; i < num; i++)
            {

                LineRender.SetVertexCount(currentPoint + 1);
                LineRender.SetPosition(currentPoint, Point[currentPoint]);
                currentPoint++;
                if (currentPoint >= Point.Length)  //已播放完毕闪电动画
                {
                    isActive = false;
                    Invoke("close", overClose);
                    break;
                }
            }
        }
    }



    void GeneratePoint()  //生成弯曲点
    {
        Point[0] = startPos;
        Point[Point.Length - 1] = endPos;

        int t = 0;
        ArrayList SavePointNum = new ArrayList();
        SavePointNum.Add(0);
        SavePointNum.Add(Point.Length - 1);
        Point[0].z = Deep;
        Point[Point.Length - 1].z = Deep;

        for(int i = 0;i<BendTimes; i++)
        {
            int times = (int)Mathf.Pow(2, t);
            for (int j = 0;j < times;j++)
            {
                Point[((int)SavePointNum[j] + (int)SavePointNum[j + 1]) / 2] = Vector2.Lerp(Point[(int)SavePointNum[j]], Point[(int)SavePointNum[j + 1]], 0.5f) + new Vector2(Random.Range(-BendScale, BendScale), Random.Range(-BendScale, BendScale)) / times;
                Point[((int)SavePointNum[j] + (int)SavePointNum[j + 1]) / 2].z = Deep;  //设置深度
                SavePointNum.Add(((int)SavePointNum[j] + (int)SavePointNum[j + 1]) / 2);
            }
            SavePointNum.Sort();
            t++;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            CharacterControl.instance.hurt(2);
        }
    }

    void close()
    {
        Thunder.instance.recovery_Lighting(this.gameObject);
    }

    void changeColliderPoint()
    {
        Vector2[] colliderPoint = new Vector2[5];
        for(int i = 0;i < 5;i++)
        {
            colliderPoint[i] = Point[Point.Length / 4 * i];
        }
        ECollider.points = colliderPoint;
    }

    private void OnDisable()
    {
        ECollider.enabled = false;
    }

}
