using UnityEngine;
using System.Collections;

public class Shiner : MonoBehaviour {
    //对于发光物，呼吸效果


    public float speed;
    public Color color;
    public SpriteRenderer[] SR;

    private Color[] originColor;
    private float timer = 0;

	void Start () {
        originColor = new Color[SR.Length];
        for(int i = 0;i<SR.Length;i++)
        {
            originColor[i] = SR[i].color;
        }

    }
	
	void Update () {
        timer += Time.deltaTime;
	    for(int i = 0;i<SR.Length;i++)
        {
            SR[i].color = Color.Lerp(Color.white, color, 0.5f * Mathf.Sin(timer * speed) + 0.5f);
        }
	}
}
