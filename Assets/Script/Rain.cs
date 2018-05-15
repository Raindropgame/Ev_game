using UnityEngine;
using System.Collections;

public class Rain : MonoBehaviour {

    //雨天

    public int MaxRaindropNum = 350;
    public float TransitionScale = 0.2f;

    private ParticleSystem ParticleSys;
    private Transform CameraTrans;

	// Use this for initialization
	void Start () {
        ParticleSys = this.GetComponent<ParticleSystem>();
        CameraTrans = GameObject.Find("MainCamera").transform;

        ParticleSystem.EmissionModule em = ParticleSys.emission;   //初始化
        em.rate = 0;
    }

    private void FixedUpdate()
    {
        this.transform.position = new Vector3(CameraTrans.position.x, CameraTrans.position.y, 0) + new Vector3(0, 15, 100);  //实时跟随相机

        if(WeatherData.getIntance().currentWeather == weather.Rain || WeatherData.getIntance().currentWeather == weather.RainAndThunder)
        {
            ParticleSystem.EmissionModule em = ParticleSys.emission;
            em.rate = getRaindropNum(WeatherData.getIntance().Weather_duration, WeatherData.getIntance().Weather_leftTime);
        }
    }

    int getRaindropNum(float duration, float leftTime)   //获得当前雨滴数量
    {
        if (leftTime > (1 - TransitionScale) * duration)
        {
            float t1 = duration - leftTime;
            float t2 = duration * TransitionScale;
            return (int)Mathf.Lerp(0, MaxRaindropNum, t1 / t2);
        }
        if (leftTime < (1 - TransitionScale) * duration && leftTime > TransitionScale * duration)
        {
            return MaxRaindropNum;
        }
        else
        {
            float t1 = TransitionScale * duration - leftTime;
            float t2 = duration * TransitionScale;
            return (int)Mathf.Lerp(MaxRaindropNum, 0, t1 / t2);
        }
    }
}
