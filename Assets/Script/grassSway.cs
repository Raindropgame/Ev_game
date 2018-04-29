using UnityEngine;
using System.Collections;

public class grassSway : MonoBehaviour {

    public float maxAngle = 10;
    public float time = 0.4f;

    private BoxCollider2D boxColl;
    private int dir;
    private float k,_time = 0;
	// Use this for initialization
	void Start () {
        boxColl = GetComponent<BoxCollider2D>();
        if(boxColl == null)
        {
            boxColl = gameObject.AddComponent<BoxCollider2D>();
            boxColl.isTrigger = true;
        }
        k = maxAngle / (time / 2);
	}

    private void FixedUpdate()
    {
        if(boxColl.enabled == false)
        {
            _time += Time.deltaTime;
            transform.eulerAngles = new Vector3(0, 0, getAngle(_time));
            if(_time>time)
            {
                boxColl.enabled = true;
                _time = 0;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.transform.name == "character")
        {
            if(collision.transform.position.x < transform.position.x)
            {
                dir = 1;
            }
            else
            {
                dir = 0;
            }
            boxColl.enabled = false;
        }
    }

    float getAngle(float t)
    {
        if(t < time/2)
        {
            return dir == 1 ? t * k : -t * k;
        }
        else
        {
            return dir == 1 ? -k * t + 2 * maxAngle : -(-k * t + 2 * maxAngle);
        }
    }

}
