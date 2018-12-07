using UnityEngine;
using System.Collections;

public class Cobweb : MonoBehaviour {

    public GameObject Line, Spider;
    public float cycle;  //周期
    public float scale; 

    private Vector3 originScale;
    private SpriteRenderer SR_Line;
    private bool isNight;

    private void Start()
    {
        Weather.becomeDay += day;
        Weather.becomeNight += night;

        originScale = Line.transform.localScale;
        SR_Line = Line.GetComponent<SpriteRenderer>();

        //获取时间状态
        if(Weather.instance.getDayState() == DayOrNight.Night)
        {
            isNight = true;
        }
        else
        {
            isNight = false;
        }
    }

    private void FixedUpdate()
    {
        //夜晚不活动
        if (!isNight)
        {
            Line.transform.localScale = Mathf.Sin(Time.time / cycle) * scale * GameFunction.getVector3(0, originScale.y, 0) + originScale;
            Spider.transform.position = SR_Line.bounds.min + GameFunction.getVector3(0, 0, 0.001f);
        }
    }

    void night()
    {
        isNight = true;
    }

    void day()
    {
        isNight = false;
    }

    private void OnDestroy()
    {
        Weather.becomeDay -= day;
        Weather.becomeNight -= night;
    }
}
