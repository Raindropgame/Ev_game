using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class showFPS : MonoBehaviour {


    float lastTime = 0;
    Text text;

    private void Start()
    {
        text = GetComponent<Text>();
    }

    private void Update()
    {
        float t = Time.time - lastTime;
        text.text = "FPS: " + ((int)(1 / t)).ToString();
        lastTime = Time.time;
    }

}
