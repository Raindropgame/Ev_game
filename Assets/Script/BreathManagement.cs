using UnityEngine;
using System.Collections;

public class BreathManagement : MonoBehaviour {

    // 气息管理

    public static BreathManagement instance;

    private void Awake()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        //过载
        if (CharacterAttribute.GetInstance().Breath_real <= 0)
        {
            CharacterAttribute.GetInstance().isOverLoad_breath = true;
        }
        //是否恢复
        if (CharacterAttribute.GetInstance().Breath_real >= CharacterAttribute.GetInstance().MaxBreath)
        {
            CharacterAttribute.GetInstance().isOverLoad_breath = false;
        }

        recoveryBreath();

    }

    void recoveryBreath()  //恢复气息
    {
        if (CharacterAttribute.GetInstance().Breath_real < CharacterAttribute.GetInstance().MaxBreath && (CharacterControl.instance.currentState == state.normal || CharacterControl.instance.currentState == state.walk) && !Scene.instance.isInit)   //初始化中不可回复
        {
            CharacterAttribute.GetInstance().Breath = CharacterAttribute.GetInstance().Breath_real + (CharacterAttribute.GetInstance().isOverLoad_breath? CharacterAttribute.GetInstance().Speed_recovery_overload : CharacterAttribute.GetInstance().Speed_recovery) * Time.deltaTime;
        }
    }

}
