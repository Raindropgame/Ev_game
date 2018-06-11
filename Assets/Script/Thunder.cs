using UnityEngine;
using System.Collections;

public class Thunder : MonoBehaviour {

    //雷天管理
    //对象池的回收 对象自行操作

    public static Thunder instance;

    public float Lighting_Odds;
    public Vector2 startPoint, endPoint; //闪电生成的区域的左上角与右下角
    public GameObject lighting;
    public int GameobjectPoolNum;
    public ArrayList LightingPool;

    [HideInInspector]
    public Transform CameraTrans;


    private void Awake()
    {
        instance = this;

        Random.InitState(Random.Range(1, 100));  //散布随机种子
        CameraTrans = GameObject.Find("MainCamera").transform;

        LightingPool = new ArrayList();
        for (int i = 0; i < GameobjectPoolNum; i++)
        {
            LightingPool.Add((GameObject)Instantiate(lighting, position: Vector2.zero, rotation: new Quaternion(0, 0, 0, 0)));
        }
    }

    private void FixedUpdate()
    {
        if (WeatherData.getIntance().currentWeather == weather.Thunder || WeatherData.getIntance().currentWeather == weather.RainAndThunder)
        {
            if (Random.value < Lighting_Odds)
            {
                getLighting().SetActive(true);
                StartCoroutine(CameraFollow.instance.shakeCamera(0.25f, 0.06f, 0.3f));  //出现闪电时镜头抖动
            }
        }
    }

    GameObject getLighting()
    {
        if(LightingPool.Count > 0)
        {
            GameObject t = LightingPool[0] as GameObject;
            LightingPool.Remove(t);
            return t;
        }
        return (GameObject)Instantiate(lighting, position: Vector3.zero, rotation: new Quaternion(0, 0, 0, 0));
    }

    public void recovery_Lighting(GameObject t)
    {
        if (LightingPool.Count >= GameobjectPoolNum)
        {
            Destroy(t);
        }
        else
        {
            LightingPool.Add(t);
            t.SetActive(false);
        }
    }

    public Vector2 getStartPos()  //获得一个随机的二维坐标
    {
        Vector2 _startPoint = (Vector2)CameraTrans.position + startPoint;
        Vector2 _endPoint = (Vector2)CameraTrans.position + endPoint;
        return new Vector2(Random.Range(_startPoint.x, _endPoint.x), Random.Range(_endPoint.y, _startPoint.y));
    }
}
