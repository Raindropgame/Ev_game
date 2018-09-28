using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class waterFall : MonoBehaviour
{

    //管理瀑布的动画

    public Texture2D tex;
    public Color color;
    public float range = 0.51f;
    public float wavelength = 7.1f;
    public float speed = 3.6f;
    public float changeScale = 1.4f;
    public float fallSpeed;

    private float origin_range, origin_speed;
    private MeshRenderer meshRenderer;
    private float sprayDuration;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        origin_range = range;
        origin_speed = speed;

        MaterialPropertyBlock t = new MaterialPropertyBlock();
        t.SetColor("_Color", color);
        t.SetFloat("_Range", range);
        t.SetFloat("_Wavelength", wavelength);
        t.SetFloat("_Speed", speed);
        t.SetTexture("_MainTex", tex);
        t.SetFloat("_fallSpeed", fallSpeed);
        meshRenderer.SetPropertyBlock(t);
    }

    public void apply()
    {
        MaterialPropertyBlock t = new MaterialPropertyBlock();
        t.SetColor("_Color", color);
        t.SetFloat("_Range", range);
        t.SetFloat("_Wavelength", wavelength);
        t.SetFloat("_Speed", speed);
        t.SetTexture("_MainTex", tex);
        t.SetFloat("_fallSpeed", fallSpeed);
        meshRenderer.SetPropertyBlock(t);
    }

    private void FixedUpdate()
    {

        if (WeatherData.getIntance().currentWeather == weather.Rain || WeatherData.getIntance().currentWeather == weather.RainAndThunder)
        {
            if (WeatherData.getIntance().Weather_leftTime > 0.8f * WeatherData.getIntance().Weather_duration)  //开始阶段
            {
                range = Mathf.Lerp(origin_range, origin_range * changeScale, (WeatherData.getIntance().Weather_duration - WeatherData.getIntance().Weather_leftTime) / 0.2f * WeatherData.getIntance().Weather_duration);
                speed = Mathf.Lerp(origin_speed, origin_speed * changeScale, (WeatherData.getIntance().Weather_duration - WeatherData.getIntance().Weather_leftTime) / 0.2f * WeatherData.getIntance().Weather_duration);
                apply();
            }
            if (WeatherData.getIntance().Weather_leftTime < 0.8f * WeatherData.getIntance().Weather_duration && WeatherData.getIntance().Weather_leftTime > 0.2f * WeatherData.getIntance().Weather_duration)  //中间
            {
                range = origin_range * changeScale;
                speed = origin_speed * changeScale;
                apply();
            }
            else  //结束
            {
                range = origin_range;
                speed = origin_speed;
                apply();
            }
        }
    }

}
