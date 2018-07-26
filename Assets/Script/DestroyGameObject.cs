using UnityEngine;
using System.Collections;

public class DestroyGameObject : MonoBehaviour {

    public float destroyTime;

    private float _time = 0;

    private void Update()
    {
        _time += Time.deltaTime;
        if(_time > destroyTime)
        {
            Destroy(this.gameObject);
        }
    }

    void _destroy()
    {
        Destroy(this.gameObject);
    }
}
