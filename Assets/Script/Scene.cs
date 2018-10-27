using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scene : MonoBehaviour {

    //每个场景开始之前调用
    static public Scene instance;

    public Transform[] BornPosition;
    [HideInInspector]
    public bool isInit = true;

    private GameObject character;
    private GameObject black;
    private float _time = 0;
    private CameraFollow_Start script2;  //初始阶段使用的摄像机跟随脚本
    private CameraFollow script1;

    private void Awake()
    {

        script1 = GameObject.Find("MainCamera").GetComponent<CameraFollow>();
        script2 = GameObject.Find("MainCamera").GetComponent<CameraFollow_Start>();

        instance = this;

        character = GameObject.Find("character");

        character.transform.position = new Vector3(BornPosition[TheSceneManager.getInstance().BornPositionNum].position.x, BornPosition[TheSceneManager.getInstance().BornPositionNum].position.y , -0.1f); // 更改位置
        character.GetComponent<CharacterControl>().enabled = false;
        black = GameObject.Find("black");

        script1.enabled = false;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        if(_time > 1)  //停留1s
        {
            black.SetActive(false);
            character.GetComponent<CharacterControl>().enabled = true;
            isInit = false;

            script1.enabled = true;
            script2.enabled = false;

            this.enabled = false;
        }
    }
}
