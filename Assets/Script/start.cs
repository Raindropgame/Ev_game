using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class start : MonoBehaviour {

    public float time;
    float totalTime;
    Image img;
	// Use this for initialization
	void Start () {
        img = this.GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
        totalTime += Time.deltaTime;
        if (totalTime > time)
        {
            img.color -= new Color(0, 0, 0, 0.03f);
            if (img.color.a <= 0)
                SceneManager.LoadScene("forest-1");
        }
	}
}
