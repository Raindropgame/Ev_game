using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BackgorundMove : MonoBehaviour {

    public EditorScript test;

    private GameObject[] maps = null;
    private GameObject[] background = null;
    private Vector3 nowPosition;
    public GameObject ss;

    private void OnEnable()
    {
        maps = GameObject.FindGameObjectsWithTag("map");
        background = GameObject.FindGameObjectsWithTag("map_background");
        for(int i = 0;i<background.Length;i++)  //防止背景抖动，直接设为摄像机子物体
        {
            background[i].transform.SetParent(this.transform, true);
        }
        nowPosition = this.transform.position;
    }

    private void Update()  //Update 背景会抖动
    {
        if (test.isEditor)  //编辑模式下运行该函数
        {
            try
            {
                //  背景z轴不可超过10
                Vector3 tVector = nowPosition - this.transform.position;
                tVector.z = 0;
                for (int i = 0; i < maps.Length; i++)
                {
                    maps[i].transform.position = new Vector3((maps[i].transform.position - tVector * (maps[i].transform.position.z * 0.1f)).x, (maps[i].transform.position - 0.2f * tVector * (maps[i].transform.position.z * 0.1f)).y, maps[i].transform.position.z);
                }
                for (int i = 0; i < background.Length; i++)
                {
                    background[i].transform.position = background[i].transform.position - tVector * (background[i].transform.position.z * 0.1f);
                }
                nowPosition = this.transform.position;
            }
            catch
            {
                maps = GameObject.FindGameObjectsWithTag("map");
                background = GameObject.FindGameObjectsWithTag("map_background");
                nowPosition = this.transform.position;
            }
        }
        else
        {
            /*Vector3 tVector = nowPosition2 - this.transform.position;
            tVector.z = 0;
            for (int i = 0; i < background.Length; i++)
            {
                if (Mathf.Abs(background[i].transform.position.z - 10) < 0.05f)  //减少背景抖动
                {
                    background[i].transform.position = background[i].transform.position - tVector;
                }
                else
                {
                    background[i].transform.position = background[i].transform.position - tVector * (background[i].transform.position.z * 0.1f);
                }
            }
            nowPosition2 = this.transform.position;*/
        }

    }

    private void FixedUpdate()  //Update 背景会抖动
    {
        //  背景z轴不可超过10
        Vector3 tVector = nowPosition - this.transform.position;
        tVector.z = 0;
        for (int i = 0; i < maps.Length; i++)
        {
            maps[i].transform.position = new Vector3((maps[i].transform.position - tVector * (maps[i].transform.position.z * 0.1f)).x, (maps[i].transform.position - 0.2f * tVector * (maps[i].transform.position.z * 0.1f)).y, maps[i].transform.position.z);
        }
        nowPosition = this.transform.position;
    }
}
