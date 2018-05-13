using UnityEngine;
using System.Collections;

public class WeatherData{

    private static WeatherData instance;

    public float currentTime = 0;
    public weather currentWeather;
    public float Weather_duration,Weather_leftTime;

    public static WeatherData getIntance()
    {
        if(instance == null)
        {
            instance = new WeatherData();
        }
        return instance;
    } 

    public void getFile()  //获取存档
    {
        //测试代码
        currentTime = 0;
        currentWeather = weather.Sunny;
        Weather_duration = 5;
        Weather_leftTime = 5;
        //---------
    }
}
