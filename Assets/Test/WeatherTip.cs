using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeatherTip : MonoBehaviour {

    private Text text;

	// Use this for initialization
	void Start () {
        text = this.GetComponent<Text>();
	}
	
	void Update () {
        text.text = ((int)WeatherData.getIntance().currentTime).ToString() + "\n" + WeatherData.getIntance().currentWeather.ToString() + "\n" + (int)WeatherData.getIntance().Weather_leftTime;
	}
}
