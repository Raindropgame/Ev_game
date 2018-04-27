using UnityEngine;
using System.Collections;

public class CharacterObjectManager : MonoBehaviour
{

    public static CharacterObjectManager instance;
    public GameObject _arrow;
    public int MaxArrowNum = 20;
    public int currentArrowNum;

    private GameObject character;
    private GameObject _attack1_left, _attack1_right, _attack2_left, _attack2_right;
    private GameObject pool_arrow; //对象池
    private ArrayList arrowList;


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

        arrowList = new ArrayList();
        for (int i = 0; i < MaxArrowNum; i++)  //初始化创建箭
        {
            GameObject t_gameobject = Instantiate(_arrow, pool_arrow.transform) as GameObject;
            t_gameobject.SetActive(false);
            arrowList.Add(t_gameobject);
        }
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


}