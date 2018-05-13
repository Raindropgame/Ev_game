using UnityEngine;
using System.Collections;

public class TheLight : MonoBehaviour {

    public float Day_Alpha = 1, Night_Alpha = 0;

    private SpriteRenderer SR;

	void Start () {
        SR = this.GetComponent<SpriteRenderer>();
	}
	

	void Update () {
        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, Weather.instance.SmoothLerp_LightAlpha(Day_Alpha,Night_Alpha));  //随时间变化透明度
	}
}
