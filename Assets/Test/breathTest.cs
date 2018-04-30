using UnityEngine;
using System.Collections;

public class breathTest : MonoBehaviour {

    private RectTransform rTrans;

	// Use this for initialization
	void Start () {
        rTrans = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        rTrans.sizeDelta = new Vector2(CharacterAttribute.GetInstance().Breath * 183 / CharacterAttribute.GetInstance().MaxBreath, 18);
        if (CharacterAttribute.GetInstance().Breath < CharacterAttribute.GetInstance().MaxBreath && (CharacterControl.instance.currentState == state.normal || CharacterControl.instance.currentState == state.walk) && !Scene.instance.isInit)   //初始化中不可回复
        {
            CharacterAttribute.GetInstance().Breath += CharacterAttribute.GetInstance().Speed_recover * Time.deltaTime;
        }
	}
}
