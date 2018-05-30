using UnityEngine;
using System.Collections;

public class HaloWave : MonoBehaviour {

    //灯光波动

    [Range(0, 1)]
    public float waveScale;
    public float waveTime; 
    

    [HideInInspector]
    public float standardRange;
    private Light _light;
    private float _time0 = 0;
    private bool t = false;

    private void Start()
    {
        _light = GetComponent<Light>();
        standardRange = _light.range;
    }

    private void Update()
    {
        _time0 += Time.deltaTime;
        if(t)
        {
            _light.range = Mathf.Lerp(standardRange, standardRange * (waveScale + 1), _time0 / waveTime);
        }
        else
        {
            _light.range = Mathf.Lerp(standardRange * (waveScale + 1), standardRange, _time0 / waveTime);
        }

        if(_time0 > waveTime)//改变状态
        {
            t = !t;
            _time0 = 0;
        }
    }
}
