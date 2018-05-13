using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pause : MonoBehaviour {

    //暂停测试脚本
    public GameObject black;

	void Update () {
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            black.SetActive(true);
            black.GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f, 0.5f);
            CharacterControl.instance.enabled = false;
            Time.timeScale = 0;
        }
        if(Input.GetKeyDown(KeyCode.Return))
        {
            black.GetComponent<Image>().color = new Color(0, 0, 0, 1);
            black.SetActive(false);
            CharacterControl.instance.enabled = true;
            Time.timeScale = 1;
        }
	}
}
