using UnityEngine;
using System.Collections;

public class WeatherData{

    private static WeatherData instance;

    public float currentTime = 0;
    public weather currentWeather;
    public float Weather_duration,Weather_leftTime;
    public float Rain_Odds = 0.2f, Thunder_Odds = 0.1f, RainAndThunder_Odds = 0.1f;   //各种天气的概率

    public static WeatherData getIntance()
    {
        if(instance == null)
        {
            instance = new WeatherData();
        }
        return instance;
    } 

    WeatherData()
    {
        Init();
    }

    public void Init()  //获取存档
    {
        currentTime = PlayerPrefs.GetFloat("currentTime", 0);
        Weather_duration = PlayerPrefs.GetFloat("Weather_duration", 5);
        Weather_leftTime = PlayerPrefs.GetFloat("Weather_leftTime", 5);
        currentWeather = (weather)PlayerPrefs.GetInt("currentWeather");
        Rain_Odds = PlayerPrefs.GetFloat("Rain_Odds");
        Thunder_Odds = PlayerPrefs.GetFloat("Thunder_Odds");
        RainAndThunder_Odds = PlayerPrefs.GetFloat("RainAndThunder_Odds");
    }

    public void setFile()  //存档
    {
        PlayerPrefs.SetFloat("currentTime", currentTime);
        PlayerPrefs.SetFloat("Weather_duration", Weather_duration);
        PlayerPrefs.SetFloat("Weather_leftTime", Weather_leftTime);
        PlayerPrefs.SetInt("currentWeather", (int)currentWeather);
        PlayerPrefs.SetFloat("Rain_Odds",Rain_Odds);
        PlayerPrefs.SetFloat("Thunder_Odds",Thunder_Odds);
        PlayerPrefs.SetFloat("RainAndThunder_Odds",RainAndThunder_Odds);
    }
}
