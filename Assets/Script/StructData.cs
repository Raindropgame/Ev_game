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
        
    
    public GameObject[] GemItem = new GameObject[3];
}
