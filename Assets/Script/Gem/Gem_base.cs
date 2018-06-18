using UnityEngine;
using System.Collections;

public class Gem_base{

    //结晶基类

    public int index;  //索引
    public Sprite sprite;  //手动添加
    public Arms attachArms;  //所镶嵌的装备

    public static Gem_base Index2Gem(int i)  //根据索引号查询结晶
    {
        Gem_base o;
        switch(i)
        {
            case 0:
                o = new Gem_fire();
                break;
            default:
                o = new Gem_base();
                break;
        }
        return o;
    }

    virtual public void OnStart()
    {

    }

    virtual public bool Take(Arms arms)  //返回是否操作成功
    {
        if(CharacterAttribute.GetInstance().ArmsGemGroove[(int)arms].getLeftGrooveNum() <= 0)  //是否有空出的槽
        {
            return false;
        }
        attachArms = arms;
        return true;
    }   //佩戴

    virtual public bool TakeOff()
    {
        return true;
    }

    virtual public void Work()  //效果
    {

    }
}
