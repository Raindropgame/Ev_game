using UnityEngine;
using System.Collections;

public class HitPoint : MonoBehaviour {

    //打击点

    public float LiveTime;

    private float _time = 0;

	
	void Update () {
        _time += Time.deltaTime;
        if(_time > LiveTime)
        {
            Destroy(this.gameObject);
        }
	}
}
