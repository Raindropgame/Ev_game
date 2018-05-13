using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class Weather : MonoBehaviour {

    //  天气系统
    // 白天 黑夜  
    //  晴天 雷天   雨天  雷雨天

    public static Weather instance;  //单例模式

    public float DayTime,TotalTime;  //昼夜时长
    public Material MaskerMaterial;
    public float Rain_Odds = 0.2f, Thunder_Odds = 0.1f, RainAndThunder_Odds = 0.1f;   //各种天气的概率

    private GameObject Light_Character = null;
    private Blur Script_Blur;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //WeatherData.getIntance().getFile();  //初始化

        Light_Character = GameObject.Find("Light_Character");
        Script_Blur = GameObject.Find("BackGroundCamera").GetComponent<Blur>();

    }

    private void FixedUpdate()
    {

        WeatherData.getIntance().currentTime += Time.deltaTime;  //更新时间
        if(WeatherData.getIntance().currentTime > TotalTime)
        {
            WeatherData.getIntance().currentTime = 0;
        }

        WeatherData.getIntance().Weather_leftTime -= Time.deltaTime;
        if(WeatherData.getIntance().Weather_leftTime < 0)   //开始下一个天气
        {
            WeatherData.getIntance().currentWeather = getNextWeather();
            WeatherData.getIntance().Weather_duration = TotalTime * Random.Range(0.1f, 0.7f);
            WeatherData.getIntance().Weather_leftTime = WeatherData.getIntance().Weather_duration;
        }

        MaskerMaterial.SetFloat("_Distance", SmoothLerp_distance());  // 更新Masker
        Script_Blur.nightColor = SmoothLerp_BackgroundCamera();   //更新背景相机


    }

    public DayOrNight getDayState()  //获得当前昼夜情况
    {
        if(WeatherData.getIntance().currentTime < DayTime)
        {
            return DayOrNight.Day;
        }
        return DayOrNight.Night;
    }

    float SmoothLerp_distance()  //获取Masker的distance值
    {
        float Day_Distance = 200, Night_distance = 18;
        float DayScale = 0.2f;  //过渡时间
        float NightTime = TotalTime - DayTime;  //夜晚的时长
        if(WeatherData.getIntance().currentTime < (1 - DayScale) * DayTime && WeatherData.getIntance().currentTime > DayScale * DayTime)  //处于白天
        {
            return Day_Distance;
        }
        if(WeatherData.getIntance().currentTime < DayTime + (1 - DayScale) * NightTime && WeatherData.getIntance().currentTime > DayTime +  DayScale * NightTime)  //处于夜晚
        {
            return Night_distance;
        }
        if(WeatherData.getIntance().currentTime > (1 - DayScale) * DayTime && WeatherData.getIntance().currentTime < DayTime + DayScale * NightTime)
        {
            float t1 = WeatherData.getIntance().currentTime - (1 - DayScale) * DayTime;
            float t2 = DayScale * DayTime + DayScale * NightTime;
            return Mathf.SmoothStep(Day_Distance, Night_distance, t1 / t2);
        }
        else
        {
            float _currentTime = WeatherData.getIntance().currentTime;
            if(_currentTime < DayTime * DayScale)
            {
                _currentTime = DayTime + NightTime + _currentTime;
            }
            float t1 = _currentTime - (DayTime + (1 - DayScale) * NightTime);
            float t2 = DayScale * DayTime + DayScale * NightTime;
            return Mathf.SmoothStep(Night_distance, Day_Distance, t1 / t2);
        }
    }


    public Color Day_Color = Color.white, Night_Color = new Color(0.4f,0.4f,0.4f,0.25f);  //白天和黑夜主色调
    Color SmoothLerp_BackgroundCamera()  //获取背景相机的色调
    {
        float DayScale = 0.2f;  //过渡时间
        float NightTime = TotalTime - DayTime;  //夜晚的时长
        if (WeatherData.getIntance().currentTime < (1 - DayScale) * DayTime && WeatherData.getIntance().currentTime > DayScale * DayTime)  //处于白天
        {
            return Day_Color;
        }
        if (WeatherData.getIntance().currentTime < DayTime + (1 - DayScale) * NightTime && WeatherData.getIntance().currentTime > DayTime + DayScale * NightTime)  //处于夜晚
        {
            return Night_Color;
        }
        if (WeatherData.getIntance().currentTime > (1 - DayScale) * DayTime && WeatherData.getIntance().currentTime < DayTime + DayScale * NightTime)
        {
            float t1 = WeatherData.getIntance().currentTime - (1 - DayScale) * DayTime;
            float t2 = DayScale * DayTime + DayScale * NightTime;
            return Color.Lerp(Day_Color, Night_Color, t1 / t2);
        }
        else
        {
            float _currentTime = WeatherData.getIntance().currentTime;
            if (_currentTime < DayTime * DayScale)
            {
                _currentTime = DayTime + NightTime + _currentTime;
            }
            float t1 = _currentTime - (DayTime + (1 - DayScale) * NightTime);
            float t2 = DayScale * DayTime + DayScale * NightTime;
            return Color.Lerp(Night_Color, Day_Color, t1 / t2);
        }
    }

    public float SmoothLerp_LightAlpha(float Day_Alpha,float Night_Alpha)  //获取Light的透明度
    {
        float DayScale = 0.2f;  //过渡时间
        float NightTime = TotalTime - DayTime;  //夜晚的时长
        if (WeatherData.getIntance().currentTime < (1 - DayScale) * DayTime && WeatherData.getIntance().currentTime > DayScale * DayTime)  //处于白天
        {
            return Day_Alpha;
        }
        if (WeatherData.getIntance().currentTime < DayTime + (1 - DayScale) * NightTime && WeatherData.getIntance().currentTime > DayTime + DayScale * NightTime)  //处于夜晚
        {
            return Night_Alpha;
        }
        if (WeatherData.getIntance().currentTime > (1 - DayScale) * DayTime && WeatherData.getIntance().currentTime < DayTime + DayScale * NightTime)
        {
            float t1 = WeatherData.getIntance().currentTime - (1 - DayScale) * DayTime;
            float t2 = DayScale * DayTime + DayScale * NightTime;
            return Mathf.SmoothStep(Day_Alpha, Night_Alpha, t1 / t2);
        }
        else
        {
            float _currentTime = WeatherData.getIntance().currentTime;
            if (_currentTime < DayTime * DayScale)
            {
                _currentTime = DayTime + NightTime + _currentTime;
            }
            float t1 = _currentTime - (DayTime + (1 - DayScale) * NightTime);
            float t2 = DayScale * DayTime + DayScale * NightTime;
            return Mathf.SmoothStep(Night_Alpha, Day_Alpha, t1 / t2);
        }
    }

    weather getNextWeather()   //随机下一个天气
    {
        Random.InitState(Random.Range(1, 100));
        float t = Random.value;
        if(t < Rain_Odds)
        {
            return weather.Rain;
        }
        if(t < Rain_Odds + Thunder_Odds)
        {
            return weather.Thunder;
        }
        if(t < Rain_Odds + Thunder_Odds + RainAndThunder_Odds)
        {
            return weather.RainAndThunder;
        }
        else
        {
            return weather.Sunny;
        }
    }

}
