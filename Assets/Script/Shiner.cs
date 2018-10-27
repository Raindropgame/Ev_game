using UnityEngine;
using System.Collections;

public class Shiner : MonoBehaviour {
    //对于发光物，呼吸效果


    public float frequency;
    public float changeScale;
    public SpriteRenderer[] SR;

    private Color[] originColor;

	void Start () {
        originColor = new Color[SR.Length];
        for(int i = 0;i<SR.Length;i++)
        {
            originColor[i] = SR[i].color;
        }

    }
	
	void Update () {
	    for(int i = 0;i<SR.Length;i++)
        {
            SR[i].color = originColor[i] + changeScale * Color.white * (float)(0.5 * Mathf.Sin(Time.time / frequency) - 1);
            SR[i].color = new Color(SR[i].color.r, SR[i].color.g, SR[i].color.b, 1);
        }
	}
}
