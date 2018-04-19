using UnityEngine;
using System.Collections;

public class BackgorundMove : MonoBehaviour {


    private GameObject[] maps = null;
    private Vector3 nowPosition;

	// Use this for initialization
	void Start () {
        maps = GameObject.FindGameObjectsWithTag("map");
        nowPosition = this.transform.position;
    }
	
	private void FixedUpdate()
    {
        //  背景z轴不可超过10
        Vector3 tVector = nowPosition - this.transform.position;
        tVector.z = 0;
        for(int i = 0;i<maps.Length;i++)
        {
            maps[i].transform.position = maps[i].transform.position - tVector * (maps[i].transform.position.z * 0.1f);
        }
        nowPosition = this.transform.position;
    }
}
