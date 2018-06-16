using UnityEngine;
using System.Collections;

public class Bag{

    //背包系统

    Bag()
    {
        GemItem[0] = new Gem_fire() as Gem_base;  //test
    }

    private static Bag instance;

    public static Bag getInstance()
    {
        if(instance == null)
        {
            instance = new Bag();
        }
        return instance;
    }

    //--结晶-----
    public Gem_base[] GemItem = new Gem_base[15];
	
    public int getLeftItem()   //获取剩下的格子数
    {
        int i = 0;
        foreach(Gem_base gem in GemItem)
        {
            if (gem == null)
            {
                i++;
            }
        }
        return i;
    }

    public bool putGemIntoBag(Gem_base Gem)  //放入
    {
        if(getLeftItem() <= 0)  //是否还有剩下的格子
        {
            return false;
        }
        for(int i = 0;i<GemItem.Length;i++)  //寻找空余的格子
        {
            if(GemItem[i] == null)
            {
                GemItem[i] = Gem;
                return true;
            }
        }
        return false;
    }
}
