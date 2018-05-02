using UnityEngine;
using System.Collections;

public class CharacterObjectManager : MonoBehaviour
{

    public static CharacterObjectManager instance;
    public GameObject _arrow,_arrow_2;
    public int MaxArrowNum = 20, MaxArrow_2Num = 20;
    public int currentArrowNum, currentArrow_2Num;

    private GameObject character;
    private GameObject _attack1_left, _attack1_right, _attack2_left, _attack2_right;
    private GameObject pool_arrow; //对象池
    private ArrayList arrowList, arrow_2List;
    private Vector3 arrow2_rotation;


    private void Start()
    {
        instance = this;
        character = GameObject.Find("character");

        _attack1_left = GameObject.Find("attack1_left");
        _attack1_right = GameObject.Find("attack1_right");
        _attack2_left = GameObject.Find("attack2_left");
        _attack2_right = GameObject.Find("attack2_right");
        _attack1_left.SetActive(false);
        _attack1_right.SetActive(false);
        _attack2_left.SetActive(false);
        _attack2_right.SetActive(false);

        pool_arrow = GameObject.Find("Pool_arrow");

        currentArrowNum = MaxArrowNum;  //初始化当前箭的数目
        currentArrow_2Num = MaxArrow_2Num;

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

        arrow2_rotation = new Vector3(0, 0, 900 - 2 * _arrow_2.transform.eulerAngles.z);  //计算方向为左需要旋转的度数
    }

    private void FixedUpdate()
    {
        this.transform.position = character.transform.position;   //跟随主角
    }

    public void attack1(dir Dir)  //播放刀影动画
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
        getArrow().SetActive(true);
    }

    public void arrow_2()
    {
        getArrow_2().SetActive(true);
    }

    public GameObject getArrow()  //从池中获取箭
    {
        GameObject t_object;
        if (currentArrowNum > 0)
        {
            t_object = arrowList[0] as GameObject;
            arrowList.Remove(t_object);
            currentArrowNum--;
        }
        else
        {
            t_object = Instantiate(_arrow, pool_arrow.transform) as GameObject;
            t_object.transform.parent = pool_arrow.transform;  //设置父物体
            arrowList.Add(t_object);
            t_object.SetActive(false);
        }
        return t_object;
    }

    public GameObject getArrow_2()  //从池中获取箭2
    {
        GameObject t_object;
        if (currentArrow_2Num > 0)
        {
            t_object = arrow_2List[0] as GameObject;
            arrow_2List.Remove(t_object);
            currentArrow_2Num--;
        }
        else
        {
            t_object = Instantiate(_arrow_2, pool_arrow.transform) as GameObject;
            t_object.transform.parent = pool_arrow.transform;  //设置父物体
            arrow_2List.Add(t_object);
            t_object.SetActive(false);
        }
        t_object.transform.position = this.transform.position + (CharacterControl.instance.Dir == dir.left ? new Vector3(-0.84f, 0.81f,-9) : new Vector3(0.94f, 0.88f,-9));  //初始化位置   改变z轴，使其在最前面
        if(CharacterControl.instance.Dir == dir.left)
        {
            t_object.transform.Rotate(arrow2_rotation);  //人物方向为左，需要旋转图片
        }
        return t_object;
    }

    public void recoveryArrow(GameObject go)  //回收箭
    {
        if (currentArrowNum >= MaxArrowNum)
        {
            arrowList.Remove(go);
            Destroy(go);
        }
        else
        {
            go.transform.parent = pool_arrow.transform;
            arrowList.Add(go);
            go.SetActive(false);
            currentArrowNum++;
        }
    }

    public void recoveryArrow_2(GameObject go)  //回收箭2
    {
        if (currentArrow_2Num >= MaxArrow_2Num)
        {
            arrow_2List.Remove(go);
            Destroy(go);
        }
        else
        {
            go.transform.parent = pool_arrow.transform;
            arrow_2List.Add(go);
            go.SetActive(false);
            currentArrow_2Num++;
        }
    }
}