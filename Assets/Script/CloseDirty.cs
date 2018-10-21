using UnityEngine;
using System.Collections;

public class CloseDirty : MonoBehaviour {

    public bool isUseUpdate = false;
    public float closeTime;

    private float _time = 0;

	public void close()
    {
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        _time += Time.deltaTime;

        if(_time > closeTime)
        {
            _time = 0;
            this.gameObject.SetActive(false);
        }
    }
}
