using UnityEngine;
using System.Collections;

public class Bag{

    //背包系统

    Bag()
    {
        GemItem[0] = new Gem_fire();  //test
        GemItem[1] = new Gem_ice();  //test
        GemItem[2] = new Gem_wood();  //test
        GemItem[3] = new Gem_lightning();  //test
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

    public int putGemIntoBag(Gem_base Gem)  //放入
    {
        if(getLeftItem() <= 0)  //是否还有剩下的格子
        {
            return -1;
        }
        for(int i = 0;i<GemItem.Length;i++)  //寻找空余的格子
        {
            if(GemItem[i] == null)
            {
                GemItem[i] = Gem;
                return i;
            }
        }
        return -1;
    }


    //武器槽碎片
    private int Arms_fragment = 0;

    public int getArmsFragment()
    {
        return Arms_fragment;
    }

    public void AddArmsFragment(int num)
    {
        Arms_fragment += num;
    }

    public void ConsumeFragment(int num)
    {
        if(num > Arms_fragment)
        {
            return;
        }
        Arms_fragment -= num;
    }
}
