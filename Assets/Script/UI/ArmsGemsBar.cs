﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArmsGemsBar : MonoBehaviour {

    //武器镶嵌UI管理

    public struct ArmsInfo
    {
        public RectTransform rectTrans;
        public string name;
        public int pos;
        public Image image;
    }

    [Header("--------武器栏属性----------")]
    public float maxArmsScale = 1.2f;
    public RectTransform sword, arrow, spear;
    public float armsMoveTime;  //移动时间
    public Color backArmsColor;  //在后面的武器的颜色
    [Header("--------武器槽栏属性----------")]
    public GameObject[] GemGroove;

    private RectTransform[] point;
    private ArrayList ArmsList = new ArrayList();
    private ArmsInfo[] armsInfo;
    private int currentArms; // 当前选中的武器

    private void Start()
    {
        //--初始化
        point = new RectTransform[3];
        point[2] = GameObject.Find("arms_point_left").GetComponent<RectTransform>();
        point[0] = GameObject.Find("arms_point_middle").GetComponent<RectTransform>();
        point[1] = GameObject.Find("arms_point_right").GetComponent<RectTransform>();

        //初始化各个武器的位置
        if(CharacterAttribute.GetInstance().isEnable[(int)state.attack1])  //检查当前有哪些武器
        {
            ArmsList.Add(sword);
        }
        else
        {
            sword.gameObject.SetActive(false);
        }
        if (CharacterAttribute.GetInstance().isEnable[(int)state.shoot])
        {
            ArmsList.Add(arrow);
        }
        else
        {
            arrow.gameObject.SetActive(false);
        }
        if (CharacterAttribute.GetInstance().isEnable[(int)state.jumpshoot])
        {
            ArmsList.Add(spear);
        }
        else
        {
            spear.gameObject.SetActive(false);
        }
        armsInfo = new ArmsInfo[ArmsList.Count];
        for(int i = 0;i<ArmsList.Count;i++)
        {
            ((RectTransform)ArmsList[i]).position = point[i].position;

            armsInfo[i].rectTrans = ((RectTransform)ArmsList[i]);
            armsInfo[i].name = ((RectTransform)ArmsList[i]).name;
            armsInfo[i].pos = i;
            armsInfo[i].image = ((RectTransform)ArmsList[i]).gameObject.GetComponent<Image>();

            if(i == 0)   //改变样式
            {
                ((RectTransform)ArmsList[i]).localScale = new Vector2(maxArmsScale, maxArmsScale);
                armsInfo[i].image.color = Color.white;

                currentArms = i;
            }
            else
            {
                ((RectTransform)ArmsList[i]).localScale = new Vector2(1, 1);
                armsInfo[i].image.color = backArmsColor;
            }
        }
    }

    private void OnDisable()
    {
        ArmsList.Clear();
        //初始化各个武器的位置
        if (CharacterAttribute.GetInstance().isEnable[(int)state.attack1])  //检查当前有哪些武器
        {
            ArmsList.Add(sword);
        }
        else
        {
            sword.gameObject.SetActive(false);
        }
        if (CharacterAttribute.GetInstance().isEnable[(int)state.shoot])
        {
            ArmsList.Add(arrow);
        }
        else
        {
            arrow.gameObject.SetActive(false);
        }
        if (CharacterAttribute.GetInstance().isEnable[(int)state.jumpshoot])
        {
            ArmsList.Add(spear);
        }
        else
        {
            spear.gameObject.SetActive(false);
        }
        armsInfo = new ArmsInfo[ArmsList.Count];
        for (int i = 0; i < ArmsList.Count; i++)
        {
            ((RectTransform)ArmsList[i]).position = point[i].position;

            armsInfo[i].rectTrans = ((RectTransform)ArmsList[i]);
            armsInfo[i].name = ((RectTransform)ArmsList[i]).name;
            armsInfo[i].pos = i;
            armsInfo[i].image = ((RectTransform)ArmsList[i]).gameObject.GetComponent<Image>();

            if (i == 0)   //改变样式
            {
                ((RectTransform)ArmsList[i]).localScale = new Vector2(maxArmsScale, maxArmsScale);
                armsInfo[i].image.color = Color.white;

                currentArms = i;
            }
            else
            {
                ((RectTransform)ArmsList[i]).localScale = new Vector2(1, 1);
                armsInfo[i].image.color = backArmsColor;
            }
        }
    }


    public void onClickArms(GameObject t)  //武器的点击事件
    {
        for(int i = 0;i<armsInfo.Length;i++)  //判断是否点击的为当前选中的武器
        {
            if(t.name == armsInfo[i].name)
            {
                if(armsInfo[i].pos == 0)
                {
                    return;   //点击的为中间的武器
                }
            }
        }
        if(t.transform.position.x > point[0].position.x)  //判断点击的位置
        {
            currentArms++;
            currentArms = currentArms > 2 ? 0 : currentArms;
            for(int i = 0;i<armsInfo.Length;i++)  //改变位置
            {
                armsInfo[i].pos--;
                if(armsInfo[i].pos < 0)
                {
                    armsInfo[i].pos = 2;
                }
            }
            StartCoroutine(moveArms(dir.left));
        }
        else
        {
            currentArms--;
            currentArms = currentArms < 0 ? 2 : currentArms;
            for (int i = 0; i < armsInfo.Length; i++)  //改变位置
            {
                armsInfo[i].pos++;
                if (armsInfo[i].pos > 2)
                {
                    armsInfo[i].pos = 0;
                }
            }
            StartCoroutine(moveArms(dir.right));
        }
    }

    IEnumerator moveArms(dir Dir)  //移动武器到当前位置
    {
        float t_time = 0;
        while(true)
        {
            t_time += Time.deltaTime;
            for(int i = 0;i<armsInfo.Length;i++)
            {
                int lastPos; //计算之前的位置
                if(Dir == dir.left)
                {
                    lastPos = armsInfo[i].pos + 1;
                    if(lastPos > 2)
                    {
                        lastPos = 0;
                    }
                }
                else
                {
                    lastPos = armsInfo[i].pos - 1;
                    if(lastPos < 0)
                    {
                        lastPos = 2;
                    }
                }

                armsInfo[i].rectTrans.position = Vector3.Slerp(point[lastPos].position, point[armsInfo[i].pos].position, t_time / armsMoveTime);  //插值位置
                if(armsInfo[i].pos == 0)    
                {
                    armsInfo[i].rectTrans.localScale = Vector2.Lerp(new Vector2(1,1), new Vector2(maxArmsScale, maxArmsScale), t_time / armsMoveTime);   //插值缩放
                    armsInfo[i].image.color = Color.Lerp(backArmsColor, new Color(1, 1, 1, 1), t_time / armsMoveTime);   //插值颜色
                }
                else
                {
                    armsInfo[i].rectTrans.localScale = Vector2.Lerp(armsInfo[i].rectTrans.localScale, new Vector2(1, 1), t_time / armsMoveTime);
                    armsInfo[i].image.color = Color.Lerp(armsInfo[i].image.color, backArmsColor, t_time / armsMoveTime);   //插值颜色
                }
                
            }

            if(t_time > armsMoveTime)  //是否结束
            {
                break;
            }
            yield return null;
        }
    }

    void updateGemGroove()  //更新武器槽的显示
    {
        ArmsGemGroove currentGemGroove;
        switch(armsInfo[currentArms].name)  //获取当前武器槽的信息
        {
            case "sword":
                currentGemGroove = CharacterAttribute.GetInstance().swordsGemGroove;
                break;
            case "arrow":
                currentGemGroove = CharacterAttribute.GetInstance().arrowGemGroove;
                break;
            case "spear":
                currentGemGroove = CharacterAttribute.GetInstance().spearGemGroove;
                break;
            default:
                currentGemGroove = CharacterAttribute.GetInstance().swordsGemGroove;
                break;
        }

        for(int i = 0;i<GemGroove.Length;i++)
        {
            //GemGroove[i].
        }
    }
}