using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour {

    //管理水面的动画

    public GameObject spray;  //水花
    public Transform surfacePos;  //水面的位置

    public Texture2D tex;
    public Color color;
    public float range = 0.51f;
    public float wavelength = 7.1f;
    public float speed = 3.6f;
    public float surface = -1;
    public float changeScale = 1.4f;

    private float origin_range, origin_speed;
    private MeshRenderer meshRenderer;
    private float sprayDuration;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        origin_range = range;
        origin_speed = speed;

        sprayDuration = spray.GetComponent<ParticleSystem>().duration;

        MaterialPropertyBlock t = new MaterialPropertyBlock();
        t.SetColor("_Color", color);
        t.SetFloat("_Range", range);
        t.SetFloat("_Wavelength", wavelength);
        t.SetFloat("_Speed", speed);
        t.SetFloat("_Surface", surface);
        t.SetTexture("_MainTex", tex);   
        meshRenderer.SetPropertyBlock(t);
    }

    public void apply()
    {
        MaterialPropertyBlock t = new MaterialPropertyBlock();
        t.SetColor("_Color", color);
        t.SetFloat("_Range", range);
        t.SetFloat("_Wavelength", wavelength);
        t.SetFloat("_Speed", speed);
        t.SetFloat("_Surface", surface);
        t.SetTexture("_MainTex", tex);
        meshRenderer.SetPropertyBlock(t);
    }

    private void FixedUpdate()
    {
        apply();
        if(WeatherData.getIntance().currentWeather == weather.Rain || WeatherData.getIntance().currentWeather == weather.RainAndThunder)
        {
            if(WeatherData.getIntance().Weather_leftTime > 0.8f * WeatherData.getIntance().Weather_duration)  //开始阶段
            {
                range = Mathf.Lerp(origin_range, origin_range * changeScale, (WeatherData.getIntance().Weather_duration - WeatherData.getIntance().Weather_leftTime) / 0.2f * WeatherData.getIntance().Weather_duration);
                speed = Mathf.Lerp(origin_speed, origin_speed * changeScale, (WeatherData.getIntance().Weather_duration - WeatherData.getIntance().Weather_leftTime) / 0.2f * WeatherData.getIntance().Weather_duration);
                apply();
            }
            if(WeatherData.getIntance().Weather_leftTime < 0.8f * WeatherData.getIntance().Weather_duration && WeatherData.getIntance().Weather_leftTime > 0.2f * WeatherData.getIntance().Weather_duration)  //中间
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject t = Instantiate(spray, position: new Vector3(collision.transform.position.x,surfacePos.position.y,transform.position.z), rotation: new Quaternion(0, 0, 0, 0)) as GameObject;
        StartCoroutine(destroySpray(t));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject t = Instantiate(spray, position: new Vector3(collision.transform.position.x, surfacePos.position.y, transform.position.z), rotation: new Quaternion(0, 0, 0, 0)) as GameObject;
        StartCoroutine(destroySpray(t));
    }

    IEnumerator destroySpray(GameObject t)  //销毁水花粒子特效
    {
        yield return new WaitForSeconds(sprayDuration);
        Destroy(t);
    }
}
