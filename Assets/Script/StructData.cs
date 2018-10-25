using UnityEngine;
using System.Collections;

public class ArmsGemGroove //单个武器的武器槽信息
{

    public ArmsGemGroove(int _currentGemNum)   //初始化槽量个数
    {
        currentGemNum = _currentGemNum;        
    }


    public ArmsGemGroove()
    {
    }

    private int m_currentGemNum = 0;
    public int currentGemNum //已开启的槽量
    {
        get
        {
            return m_currentGemNum;
        }
        set
        {
            m_currentGemNum = value;
            if(m_currentGemNum > 3)   //最大槽量为3
            {
                m_currentGemNum = 3;
            }
        }
    }


    public Gem_base[] GemItem = new Gem_base[3] { null, null, null };

    public int getLeftGrooveNum()  //获取当前还剩多少个空槽
    {
        int leftNum = 0;
        for(int i = 0;i<currentGemNum;i++)
        {
            if(GemItem[i] == null)
            {
                leftNum++;
            }
        }
        return leftNum;
    }

    public int getLastGemIndex()  //获取最后一个结晶在槽中的序号
    {
        for(int i = 0;i<GemItem.Length;i++)
        {
            if (i < GemItem.Length - 1)
            {
                if (GemItem[i] != null && GemItem[i + 1] == null)
                    return i;
            }
            else
            {
                if (GemItem[i] != null)
                    return i;
            }
        }
        return -1;
    }

    public void GemWork()   //调用结晶上的实时运行方法
    {
        for(int i = 0;i<currentGemNum;i++)
        {
            if (GemItem[i] != null)
            {
                GemItem[i].Work();
            }
        }
    }

    public int takeOn(Gem_base gem)   //装备结晶
    {
        if(getLeftGrooveNum() <= 0)  //无多余的槽
        {
            return -1;
        }

        for(int i = 0;i<GemItem.Length;i++)
        {
            if(GemItem[i] == null)
            {
                GemItem[i] = gem;  //放入
                return i;
            }
        }
        return -1;
    }
}
