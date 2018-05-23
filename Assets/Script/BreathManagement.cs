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
        recoveryBreath();
    }

    void recoveryBreath()  //恢复气息
    {
        if (CharacterAttribute.GetInstance().Breath < CharacterAttribute.GetInstance().MaxBreath && (CharacterControl.instance.currentState == state.normal || CharacterControl.instance.currentState == state.walk) && !Scene.instance.isInit)   //初始化中不可回复
        {
            CharacterAttribute.GetInstance().Breath += CharacterAttribute.GetInstance().Speed_recovery * Time.deltaTime;
        }
    }
}
