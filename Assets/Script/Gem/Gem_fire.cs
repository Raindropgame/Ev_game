using UnityEngine;
using System.Collections;

public class Gem_fire : Gem_base {
    
    //将当前的武器改为火属性

    public Gem_fire()
    {
        index = 0;
        sprite = Resources.LoadAll<Sprite>("Gem")[index];
    }

    const Attribute attribute = Attribute.fire;

    public override void Work()
    {
        base.Work();
        CharacterAttribute.GetInstance().ArmsAttribute[(int)attachArms] = attribute;
    }

    public override bool TakeOff(Arms arms)
    {
        if(base.TakeOff(arms) == false)
        {
            return false;
        }

        CharacterAttribute.GetInstance().ArmsAttribute[(int)attachArms] = Attribute.normal;
        return true;
    }
}
