using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class breathTest : MonoBehaviour {

    public Color overload_min,overload_max,normal;

    private RectTransform rTrans;
    private Image Img;

	// Use this for initialization
	void Start () {
        rTrans = GetComponent<RectTransform>();
        Img = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        if(CharacterAttribute.GetInstance().isOverLoad_breath)
        {
            Img.color = Color.Lerp(overload_min, overload_max, CharacterAttribute.GetInstance().Breath_real / CharacterAttribute.GetInstance().MaxBreath);
        }
        else
        {
            Img.color = normal;
        }

        rTrans.sizeDelta = GameFunction.getVector2(CharacterAttribute.GetInstance().Breath_real * 183 / CharacterAttribute.GetInstance().MaxBreath, 18);
	}
}
