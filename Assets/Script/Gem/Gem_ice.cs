using UnityEngine;
using System.Collections;

public class Gem_ice : Gem_base
{

    //将当前的武器改为火属性

    public Gem_ice()
    {
        index = 1;
        sprite = Resources.LoadAll<Sprite>("Gem")[index];
    }

    const Attribute attribute = Attribute.ice;

    public override void Work()
    {
        base.Work();
        CharacterAttribute.GetInstance().ArmsAttribute[(int)attachArms] = attribute;
    }

    public override bool TakeOff()
    {
        if (base.TakeOff() == false)
        {
            return false;
        }

        CharacterAttribute.GetInstance().ArmsAttribute[(int)attachArms] = Attribute.normal;
        return true;
    }
}