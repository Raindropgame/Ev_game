using UnityEngine;
using System.Collections;

public class CharacterObjectManager : MonoBehaviour
{

    //委托----
    public delegate void sendHurtEventHandler(int damage, Attribute attribute, int gameobjectID);
    public static event sendHurtEventHandler _sendHurt;
    //-------

    public static CharacterObjectManager instance;
    public GameObject _arrow,_arrow_2,hitPoint;
    public int MaxArrowNum = 20, MaxArrow_2Num = 20 , MaxHitPoint = 4;
    public GameObject shootArrowParticleObject;  //射击粒子特效
    public GameObject dashParticleObject;  //冲刺粒子特效
    public GameObject arrow_end;  //箭的击中特效
    public ParticleSystem WalkDust,WalkDust_rain;
    public ParticleSystem JumpDust,JumpDust_rain;
    public ParticleSystem FallDust, FallDust_rain;

    private GameObject character;
    private GameObject _attack1_left, _attack1_right, _attack2_left, _attack2_right;
    private GameObject pool_arrow; //对象池
    private ArrayList arrowList, arrow_2List , HitPointList = new ArrayList();
    private float shootArrowParticleLifeTime;  //射击粒子特效生命周期
    private ParticleSystem.EmissionModule dashParticle;  //冲刺粒子系统
    private ParticleSystem.EmissionModule WalkDustEmission, WalkDustEmission_rain;


    private void Start()
    {
        instance = this;
        character = GameObject.Find("character");
        shootArrowParticleLifeTime = shootArrowParticleObject.GetComponent<ParticleSystem>().startLifetime;
        dashParticle = dashParticleObject.GetComponent<ParticleSystem>().emission;
        WalkDustEmission = WalkDust.emission;
        WalkDustEmission_rain = WalkDust_rain.emission;

        _attack1_left = GameObject.Find("attack1_left");
        _attack1_right = GameObject.Find("attack1_right");
        _attack2_left = GameObject.Find("attack2_left");
        _attack2_right = GameObject.Find("attack2_right");
        _attack1_left.SetActive(false);
        _attack1_right.SetActive(false);
        _attack2_left.SetActive(false);
        _attack2_right.SetActive(false);

        pool_arrow = GameObject.Find("Pool_arrow");

        arrowList = new ArrayList();
        arrow_2List = new ArrayList();

        for (int i = 0; i < MaxArrowNum; i++)  //初始化创建箭
        {
            GameObject t_gameobject = Instantiate(_arrow, pool_arrow.transform) as GameObject;
            t_gameobject.SetActive(false);
            arrowList.Add(t_gameobject);
        }
        for (int i = 0; i < MaxArrow_2Num; i++)  //初始化创建箭2
        {
            GameObject t_gameobject = Instantiate(_arrow_2,position:pool_arrow.transform.position,rotation:_arrow_2.transform.localRotation) as GameObject;
            t_gameobject.SetActive(false);
            arrow_2List.Add(t_gameobject);
        }
        for(int i = 0;i<MaxHitPoint;i++)
        {
            GameObject t_gameobject = Instantiate(hitPoint, position: Vector3.zero, rotation: new Quaternion(0, 0, 0, 0)) as GameObject;
            t_gameobject.SetActive(false);
            HitPointList.Add(t_gameobject); 
        }

    }

    private void FixedUpdate()
    {
        this.transform.position = character.transform.position;   //跟随主角
    }

    public void attack1(dir Dir)  //刀影
    {
        if (Dir == dir.left)
        {
            if (!_attack1_left.activeSelf)
            {
                _attack1_left.SetActive(true);
            }
        }
        else
        {
            if (!_attack1_right.activeSelf)
            {
                _attack1_right.SetActive(true);
            }
        }
    }

    public void attack2(dir Dir)
    {
        if (Dir == dir.left)
        {
            if (!_attack2_left.activeSelf)
            {
                _attack2_left.SetActive(true);
            }
        }
        else
        {
            if (!_attack2_right.activeSelf)
            {
                _attack2_right.SetActive(true);
            }
        }
    }

    public void arrow()
    {
        StartCoroutine(enableShootParticle());
        getArrow().SetActive(true);
    }

    public void arrow_2()
    {
        StartCoroutine(enableShootParticle());
        getArrow_2().SetActive(true);
    }

    public void dash()  //冲刺
    {
        StartCoroutine(enableDashParticle());
    }

    //对象池操作----------

    public GameObject getArrow()  //从池中获取箭
    {
        GameObject t_object;
        if (arrowList.Count > 0)
        {
            t_object = arrowList[0] as GameObject;
            arrowList.Remove(t_object);
        }
        else
        {
            t_object = Instantiate(_arrow, pool_arrow.transform) as GameObject;
            t_object.SetActive(false);
        }
        return t_object;
    }

    public GameObject getArrow_2()  //从池中获取箭2
    {
        GameObject t_object;
        if (arrow_2List.Count > 0)
        {
            t_object = arrow_2List[0] as GameObject;
            arrow_2List.Remove(t_object);
        }
        else
        {
            t_object = Instantiate(_arrow_2, pool_arrow.transform) as GameObject;
            t_object.SetActive(false);
        }
        t_object.transform.position = this.transform.position + (CharacterControl.instance.Dir == dir.left ? new Vector3(-0.84f, 0.81f,-9) : new Vector3(0.94f, 0.88f,-9));  //初始化位置   改变z轴，使其在最前面
        t_object.transform.rotation = Quaternion.Euler(0,0,CharacterControl.instance.Dir == dir.left?-142:-37);
        return t_object;
    }

    public GameObject getHitPoint()  //从池中获取打击点
    {
        GameObject t;
        if(HitPointList.Count > 0)
        {
            t = HitPointList[0] as GameObject;
            HitPointList.Remove(t);
        }
        else
        {
            t = Instantiate(hitPoint, position: Vector3.zero, rotation: new Quaternion(0, 0, 0, 0)) as GameObject;
        }
        return t;
    }

    public void recoveryArrow(GameObject go)  //回收箭
    {
        if (arrowList.Count >= MaxArrowNum)
        {
            Debug.Log(arrowList.Count);
            Destroy(go);
        }
        else
        {
            go.transform.parent = pool_arrow.transform;
            arrowList.Add(go);
            go.SetActive(false);
        }
    }

    public void recoveryArrow_2(GameObject go)  //回收箭2
    {
        if (arrow_2List.Count >= MaxArrow_2Num)
        {
            Destroy(go);
        }
        else
        {
            go.transform.parent = pool_arrow.transform;
            arrow_2List.Add(go);
            go.SetActive(false);
        }
    }

    public void recovery_HitPoint(GameObject go)  //回收打击点
    {
        if(HitPointList.Count >= MaxHitPoint)
        {
            Destroy(go);
        }
        else
        {
            HitPointList.Add(go);
            go.SetActive(false);
        }
    }

    //----------------------------

    IEnumerator enableShootParticle()  //激活射击粒子特效并自动关闭
    {
        shootArrowParticleObject.transform.localScale = CharacterControl.instance.Dir == dir.left ? new Vector3(1, 1, -1) : new Vector3(1, 1, 1);  //改变方向
        shootArrowParticleObject.transform.localEulerAngles = new Vector3(CharacterControl.instance.currentState == state.jumpshoot ? (CharacterControl.instance.Dir == dir.left ?45:-40) : 0, -90, 90);  //根据跳射改变旋转角度
        shootArrowParticleObject.SetActive(true);
        yield return new WaitForSeconds(shootArrowParticleLifeTime);
        shootArrowParticleObject.SetActive(false);
    }

    IEnumerator enableDashParticle()   //激活冲刺粒子特效并自动关闭
    {
        dashParticleObject.transform.localScale = new Vector3(dashParticleObject.transform.localScale.x, CharacterControl.instance.Dir == dir.left ? 1 : -1, dashParticleObject.transform.localScale.z);  //改变方向
        dashParticle.enabled = true;
        yield return new WaitForSeconds(CharacterControl.instance.DashTime);
        dashParticle.enabled = false;
    }

    public void sendHurt(int damage,Attribute attribute,int gameobejctID)   //玩家发出的伤害
    {
        if (_sendHurt != null)  //不为空执行
        {
            _sendHurt(damage, attribute, gameobejctID);
        }
    }

    public void sendHurt_other(int damage, Attribute attribute, int gameobejctID)   //其他发出的伤害
    {
        if (_sendHurt != null)  //不为空执行
        {
            _sendHurt(damage, attribute, gameobejctID);
        }
    }

    public void changeDustParticle(state currentState, state lastState, bool isSky, bool isInRain, dir Dir)
    {
        const int walkParticleNum = 3;
        const int runParticleNum = 10;
        const int walkParticleNum_rain = 10;
        const int runParticleNum_rain = 20;
        if(lastState == currentState)  //未改变
        {
            if(isInRain)  //根据是否在雨中来改变粒子效果
            {
                switch(currentState)
                {
                    case state.run:
                        WalkDustEmission_rain.rate = runParticleNum_rain;
                        WalkDustEmission.rate = 0;
                        break;
                    case state.walk:
                        WalkDustEmission_rain.rate = walkParticleNum_rain;
                        WalkDustEmission.rate = 0;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (currentState)
                {
                    case state.run:
                        WalkDustEmission_rain.rate = 0;
                        WalkDustEmission.rate = runParticleNum;
                        break;
                    case state.walk:
                        WalkDustEmission_rain.rate = 0;
                        WalkDustEmission.rate = walkParticleNum;
                        break;
                    default:
                        break;
                }
            }
        }       
        else  //已改变
        {
            if (currentState == state.run || currentState == state.walk)
            {
                if(lastState != state.run && lastState != state.walk)  //打开粒子
                {
                    switch(currentState)  //根据状态改变粒子尘土数量
                    {
                        case state.run:
                            if (isInRain)  //判断是否在雨中
                            {
                                WalkDustEmission_rain.rate = runParticleNum_rain;
                            }
                            else
                            {
                                WalkDustEmission.rate = runParticleNum;
                            }
                            break;
                        case state.walk:
                            if (isInRain)  //判断是否在雨中
                            {
                                WalkDustEmission_rain.rate = walkParticleNum_rain;
                            }
                            else
                            {
                                WalkDustEmission.rate = walkParticleNum;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                if(lastState == state.run || lastState == state.walk)  //关闭粒子
                {
                    WalkDustEmission.rate = 0;
                    WalkDustEmission_rain.rate = 0;
                }

                if(currentState == state.jump && !isSky)  //起跳粒子
                {
                    if (!isInRain)
                    {
                        JumpDust.Stop();
                        JumpDust.Play();
                    }
                    else
                    {
                        JumpDust_rain.Stop();
                        JumpDust_rain.Play();
                    }
                }

                if(lastState == state.fall && !isSky)   //落地粒子
                {
                    if (!isInRain)  //判断是否在雨中
                    {
                        FallDust.Stop();
                        FallDust.Play();
                    }
                    else
                    {
                        FallDust_rain.Stop();
                        FallDust_rain.Play();
                    }
                }
            }

        }
    }
}