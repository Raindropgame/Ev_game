﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class Weather : MonoBehaviour {

    //  天气系统
    // 白天 黑夜  
    //  晴天 雷天   雨天  雷雨天

    //---委托
    public delegate void OnWeatherChange();
    public static event OnWeatherChange onWeatherChange;
    public static event OnWeatherChange becomeDay, becomeNight;
    //------

    public static Weather instance;  //单例模式

    public float DayTime,TotalTime;  //昼夜时长
    public Material MaskerMaterial;
    [Header("是否时间影响遮罩")]
    public bool isMasker = true;

    private Blur Script_Blur;
    private Bloom Bloom_script;
    private DayOrNight currentState;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //WeatherData.getIntance().getFile();  //初始化

        Script_Blur = GameObject.Find("BackGroundCamera").GetComponent<Blur>();
        Bloom_script = Screen1_render.instance.GetComponent<Bloom>();
        
        if(WeatherData.getIntance().currentTime < DayTime)
        {
            currentState = DayOrNight.Day;
        }
        
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

            if (onWeatherChange != null)
            {
                onWeatherChange();
            }
        }

        if (isMasker)
        {
            MaskerMaterial.SetFloat("_Alpha", SmoothLerp_distance());  // 更新Masker
        }
        Script_Blur.nightColor = SmoothLerp_BackgroundCamera();   //更新背景相机

        changeCameraThreshold();

        //昼夜情况
        if(WeatherData.getIntance().currentTime < DayTime)
        {
            if(currentState == DayOrNight.Night)
            {
                currentState = DayOrNight.Day;
                if(becomeDay != null)
                {
                    becomeDay();
                }
            }
        }
        else
        {
            if(currentState == DayOrNight.Day)
            {
                currentState = DayOrNight.Night;
                if(becomeNight != null)
                {
                    becomeNight();
                }
            }
        }
    }

    public DayOrNight getDayState()  //获得当前昼夜情况
    {
        if (WeatherData.getIntance().currentTime < DayTime)
        {
            return DayOrNight.Day;
        }
        return DayOrNight.Night;
    }

    float SmoothLerp_distance()  //获取Masker的distance值
    {
        float Day_Distance = 0, Night_distance = 1;
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
        Random.InitState(System.DateTime.Now.Millisecond);
        float t = Random.value;
        if(t < WeatherData.getIntance().Rain_Odds)
        {
            return weather.Rain;
        }
        if(t < WeatherData.getIntance().Rain_Odds + WeatherData.getIntance().Thunder_Odds)
        {
            return weather.Thunder;
        }
        if(t < WeatherData.getIntance().Rain_Odds + WeatherData.getIntance().Thunder_Odds + WeatherData.getIntance().RainAndThunder_Odds)
        {
            return weather.RainAndThunder;
        }
        else
        {
            return weather.Sunny;
        }
    }

    public bool isRain()  //当前天气是否为下雨
    {
        if(WeatherData.getIntance().currentWeather == weather.Rain || WeatherData.getIntance().currentWeather == weather.RainAndThunder)
        {
            return true;
        }
        return false;
    }

    void changeCameraThreshold()
    {
        const float originThreshold = 0.5f;
        const float scale = 0.6f;
        if(WeatherData.getIntance().currentWeather == weather.Rain || WeatherData.getIntance().currentWeather == weather.RainAndThunder)
        {
            if(WeatherData.getIntance().Weather_leftTime > 0.8f)  //开始
            {
                Bloom_script.bloomIntensity = originThreshold *  Mathf.Lerp(1, scale, (WeatherData.getIntance().Weather_duration - WeatherData.getIntance().Weather_leftTime) / (0.2f * WeatherData.getIntance().Weather_duration));
            }
            else
            {
                if(WeatherData.getIntance().Weather_leftTime < 0.2f)  //结束
                {
                    Bloom_script.bloomIntensity = originThreshold * Mathf.Lerp(scale, 1, (WeatherData.getIntance().Weather_leftTime) / (0.2f * WeatherData.getIntance().Weather_duration));
                }
                else   //持续
                {
                    Bloom_script.bloomIntensity = originThreshold * scale;
                }
            }
        }
    }

}
