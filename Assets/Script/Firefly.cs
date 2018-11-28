using UnityEngine;
using System.Collections;

public class Firefly : MonoBehaviour {

    public int Rate = 3;

    private ParticleSystem.EmissionModule PS_E;

    private void Awake()
    {
        Weather.becomeDay += becomeDay;
        Weather.becomeNight += becomeNight;
    }

    private void Start()
    {
        PS_E = GetComponent<ParticleSystem>().emission;

        if(Weather.instance.getDayState() == DayOrNight.Night)
        {
            if(WeatherData.getIntance().currentWeather == weather.Sunny)
            {
                PS_E.rate = Rate;
            }
        }
    }

    void becomeDay()
    {
        PS_E.rate = 0;
    }

    void becomeNight()
    {
        //晴朗夜晚出现
        if (WeatherData.getIntance().currentWeather == weather.Sunny)
        {
            PS_E.rate = Rate;
        }
    }

    private void OnDestroy()
    {
        Weather.becomeDay -= becomeDay;
        Weather.becomeNight -= becomeNight;
    }
}
