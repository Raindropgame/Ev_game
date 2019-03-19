using UnityEngine;
using System.Collections;

public class TheLight : MonoBehaviour {

    public float Day_Alpha = 1, Night_Alpha = 0;

    private SpriteRenderer SR;
    private float deltaMinus = 0;  //其他天气的影响值
    private weather lastWeather;
    private float _time0 = 0;
    private int random;
    private float flashSpeed = 100;
    private Texture2D noiseTex;

    void Start () {
        SR = this.GetComponent<SpriteRenderer>();
        lastWeather = WeatherData.getIntance().currentWeather;
        random = Random.Range(0, 1000);
        noiseTex = ResourcesManagement.getInstance().getResources<Texture2D>("noise_readable");
	}
	

	void Update () {
        SR.color = GameFunction.getColor(SR.color.r, SR.color.g, SR.color.b, Weather.instance.SmoothLerp_LightAlpha(Day_Alpha,Night_Alpha) - deltaMinus);  //随时间变化透明度

        if (WeatherData.getIntance().currentWeather != lastWeather && WeatherData.getIntance().currentWeather != weather.Sunny && lastWeather == weather.Sunny)  //当天气不为晴天的一刻
        {
            deltaMinus = 0;
            _time0 = 0;
        }
        if (WeatherData.getIntance().currentWeather != lastWeather && WeatherData.getIntance().currentWeather == weather.Sunny)  //当天气为晴天的一刻
        {
            deltaMinus = 1;
            _time0 = 0;
        }        
        if(WeatherData.getIntance().currentWeather != weather.Sunny)
        {
            _time0 += Time.deltaTime;
            deltaMinus = Mathf.Lerp(0, 1, _time0 / (WeatherData.getIntance().Weather_duration * 0.1f));
        }
        if (WeatherData.getIntance().currentWeather == weather.Sunny)
        {
            _time0 += Time.deltaTime;
            deltaMinus = Mathf.Lerp(1, 0, _time0 / (WeatherData.getIntance().Weather_duration * 0.1f));
        }

        GameFunction.m_Color = SR.color;
        GameFunction.m_Color.a *= (1 + Mathf.Lerp(-0.8f,0.1f, noiseTex.GetPixel(2,((int)(Time.time * flashSpeed) + random) % noiseTex.width).r * 0.5f + 0.25f));
        SR.color = GameFunction.m_Color;

        lastWeather = WeatherData.getIntance().currentWeather;
	}


}
