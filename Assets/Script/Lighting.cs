using UnityEngine;
using System.Collections;

public class Lighting : MonoBehaviour {

    //随机生成闪电

    public Vector3 startPos, endPos;  //起始点
    public float BendScale;  //弯曲程度
    public int BendTimes;  //弯曲次数

    private LineRenderer LineRender;
    private Vector3[] Point;

    private void Start()
    {
        Point = new Vector3[1 + (int)Mathf.Pow(2,BendTimes)];
        LineRender = this.GetComponent<LineRenderer>();
        LineRender.SetVertexCount(Point.Length);

        InvokeRepeating("DrawLighting", 0,0.05f);  //开启定时器
    }



    void GeneratePoint()  //生成弯曲点
    {
        Point[0] = startPos;
        Point[Point.Length - 1] = endPos;

        int t = 0;
        ArrayList SavePointNum = new ArrayList();
        SavePointNum.Add(0);
        SavePointNum.Add(Point.Length - 1);

        for(int i = 0;i<BendTimes; i++)
        {
            int times = (int)Mathf.Pow(2, t);
            for (int j = 0;j < times;j++)
            {
                Point[((int)SavePointNum[j] + (int)SavePointNum[j + 1]) / 2] = Vector2.Lerp(Point[(int)SavePointNum[j]], Point[(int)SavePointNum[j + 1]], 0.5f) + new Vector2(Random.Range(-BendScale, BendScale), Random.Range(-BendScale, BendScale)) / times;
                SavePointNum.Add(((int)SavePointNum[j] + (int)SavePointNum[j + 1]) / 2);
            }
            SavePointNum.Sort();
            t++;
        }
    }

    void DrawLighting()
    {
        GeneratePoint();
        LineRender.SetPositions(Point);
    }

}
