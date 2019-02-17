using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

    public float turnTime;
    public float duration;
    public Transform crystal;
    public Transform[] sphere;
    public SpriteRenderer SR_crystal;
    public Color laserColor;
    public LineRenderer[] LR;
    public Transform[] point;
    public Transform[] hit;
    public EdgeCollider2D[] edgeColl;
    [Header("发射前等待")]
    public float shootWaitTime;
    [Header("光环变大")]
    public float time_1;
    [Header("蓄力")]
    public float time_2;
    [Header("发射前摇")]
    public float time_3;
    [Header("发射")]
    public float time_4;
    [Header("发射后摇")]
    public float time_5;

    private bool isWork = true;
    private float Timer_turnTime = 0, Timer_duration = 0;
    private Quaternion currentQua;
    private bool isLaser = false;
    private Vector3[] originScale_sphere;
    private Vector3 originScale_hit;
    private Vector2[] t_vec2Array = new Vector2[2];

	void Start () {
        originScale_sphere = new Vector3[sphere.Length];
        for(int i = 0;i<sphere.Length;i++)
        {
            originScale_sphere[i] = sphere[i].localScale;
        }
        originScale_hit = hit[0].localScale;
	}
	
	void Update () {
	    if(isWork)
        {
            Timer_duration += Time.deltaTime;

            if(Timer_duration > duration)
            {
                Timer_turnTime += Time.deltaTime;
                float t = Timer_turnTime / turnTime;
                isLaser = false;

                //旋转
                crystal.rotation = Quaternion.Lerp(currentQua, Quaternion.Euler(currentQua.eulerAngles + Vector3.back * 90), t);

                if(Timer_turnTime > turnTime)
                {
                    Timer_duration = 0;
                    Timer_turnTime = 0;
                }
            }
            else
            {
                currentQua = crystal.rotation;
                //发射激光
                if(!isLaser)
                {
                    StartCoroutine(IE_laser());
                }
            }
        }
	}

    IEnumerator IE_laser()
    {
        isLaser = true;
        const float cycle = 40;

        yield return new WaitForSeconds(shootWaitTime);

        sphere[0].gameObject.SetActive(true);
        sphere[1].gameObject.SetActive(true);

        float timer_1 = 0;
        while(timer_1 < time_1)
        {
            timer_1 += Time.deltaTime;
            float t = timer_1 / time_1;

            sphere[0].localScale = Vector3.Lerp(Vector3.zero, originScale_sphere[0], t);
            sphere[1].localScale = Vector3.Lerp(Vector3.zero, originScale_sphere[1], t);
            yield return null;
        }

        float timer_2 = 0;
        while(timer_2 < time_2)
        {
            timer_2 += Time.deltaTime;

            sphere[0].localScale = originScale_sphere[0] * (-Mathf.Cos(timer_2 * cycle * 0.7f) * 0.1f + 1);
            sphere[1].localScale = originScale_sphere[1] * (-Mathf.Cos(timer_2 * cycle * 0.7f) * 0.1f + 1);
            yield return null;
        }

        float timer_3 = 0;
        while(timer_3 < time_3)
        {
            timer_3 += Time.deltaTime;
            float t = timer_3 / time_3;

            sphere[0].localScale = Vector3.Lerp(sphere[0].localScale, originScale_sphere[0] * 2, t);
            sphere[1].localScale = Vector3.Lerp(sphere[1].localScale, originScale_sphere[1] * 2, t);
            SR_crystal.color = Color.Lerp(laserColor, Color.white,t);
            yield return null;
        }

        float timer_4 = 0;

        LR[0].gameObject.SetActive(true);
        LR[0].SetPosition(0, point[0].position);
        hit[0].gameObject.SetActive(true);
        hit[0].transform.position = getRayPos(point[0].position, crystal.rotation.eulerAngles.z + 90);
        LR[0].SetPosition(1, hit[0].transform.position);
        LR[1].gameObject.SetActive(true);
        LR[1].SetPosition(0, point[1].position);
        hit[1].gameObject.SetActive(true);
        hit[1].transform.position = getRayPos(point[1].position, crystal.rotation.eulerAngles.z - 90);
        LR[1].SetPosition(1, hit[1].transform.position);
        edgeColl[0].enabled = true;
        t_vec2Array[0] = Vector3.zero;
        t_vec2Array[1] = hit[0].transform.localPosition;
        edgeColl[0].points = t_vec2Array;
        edgeColl[1].enabled = true;
        t_vec2Array[1] = hit[1].transform.localPosition;
        edgeColl[1].points = t_vec2Array;
        while (timer_4 < time_4)
        {
            timer_4 += Time.deltaTime;

            sphere[0].localScale = originScale_sphere[0] * (-Mathf.Cos(timer_4 * cycle) * 0.1f + 2);
            sphere[1].localScale = originScale_sphere[1] * (-Mathf.Cos(timer_4 * cycle) * 0.1f + 2);
            hit[0].localScale = originScale_hit * (-Mathf.Cos(timer_4 * cycle * 1.1f) * 0.1f + 1);
            hit[1].localScale = originScale_hit * (-Mathf.Cos(timer_4 * cycle * 1.1f + 0.1f) * 0.1f + 1);
            yield return null;
        }
        LR[0].gameObject.SetActive(false);
        LR[1].gameObject.SetActive(false);
        hit[0].gameObject.SetActive(false);
        hit[1].gameObject.SetActive(false);
        edgeColl[0].enabled = false;
        edgeColl[1].enabled = false;

        float timer_5 = 0;
        while(timer_5 < time_5)
        {
            timer_5 += Time.deltaTime;
            float t = timer_5 / time_5;

            sphere[0].localScale = Vector3.Lerp(sphere[0].localScale, Vector3.zero, t);
            sphere[1].localScale = Vector3.Lerp(sphere[1].localScale, Vector3.zero, t);
            yield return null;
        }
        sphere[0].gameObject.SetActive(false);
        sphere[1].gameObject.SetActive(false);
    }

    Vector3 getRayPos(Vector2 startPos,float angle)
    {
        int mask = 1 << LayerMask.NameToLayer("terrain");
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        RaycastHit2D hitPoint = Physics2D.Raycast(startPos, dir, 9999, mask);

        if(hitPoint.transform.Equals(null))
        {
            return startPos + dir * 999;
        }
        return GameFunction.getVector3(hitPoint.point.x,hitPoint.point.y,point[0].position.z);
    }
}
