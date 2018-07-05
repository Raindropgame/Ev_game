using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour {

    //管理水面的动画

    public Texture2D tex;
    public Color color;
    [Range(0,1)]
    public float Alpha = 0.4f;
    public float range = 0.51f;
    public float wavelength = 7.1f;
    public float speed = 3.6f;
    public float surface = -1;

    private void Start()
    {
        MaterialPropertyBlock t = new MaterialPropertyBlock();
        t.SetColor("_Color", color);
        t.SetFloat("_Alpha", Alpha);
        t.SetFloat("_Range", range);
        t.SetFloat("_Wavelength", wavelength);
        t.SetFloat("_Speed", speed);
        t.SetFloat("_Surface", surface);
        t.SetTexture("_MainTex", tex);   
        GetComponent<MeshRenderer>().SetPropertyBlock(t);
    }

    public void apply()
    {
        MaterialPropertyBlock t = new MaterialPropertyBlock();
        t.SetColor("_Color", color);
        t.SetFloat("_Alpha", Alpha);
        t.SetFloat("_Range", range);
        t.SetFloat("_Wavelength", wavelength);
        t.SetFloat("_Speed", speed);
        t.SetFloat("_Surface", surface);
        t.SetTexture("_MainTex", tex);
        GetComponent<MeshRenderer>().SetPropertyBlock(t);
    }

}
