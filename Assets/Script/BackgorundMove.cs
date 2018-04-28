using UnityEngine;
using System.Collections;

public class BackgorundMove : MonoBehaviour {

    [ExecuteInEditMode]
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
            maps[i].transform.position = new Vector3((maps[i].transform.position - tVector * (maps[i].transform.position.z * 0.1f)).x, (maps[i].transform.position - 0.2f * tVector * (maps[i].transform.position.z * 0.1f)).y, maps[i].transform.position.z);
        }
        nowPosition = this.transform.position;
    }
}
