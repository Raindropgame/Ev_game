using UnityEngine;
using System.Collections;

public class Sway : MonoBehaviour {

    public float sway_X = 0.05f,sway_Y = 0.02f;
    public string[] TriggerTags = { "Player" };

    private Vector3 thisPosition;
    private bool isTrigger = false;

    private void Start()
    {
        thisPosition = this.transform.position;
    }

    private void Update()
    {
        if(!isTrigger)
        {
            this.transform.position = thisPosition;
        }
        isTrigger = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        for(int i = 0;i<TriggerTags.Length;i++)
        {
            if(collision.transform.tag == TriggerTags[i])
            {
                isTrigger = true;
                sway();
                break;
            }
        }
    }

    void sway()
    {
        this.transform.localPosition = new Vector3(Random.Range(0.1f, sway_X) * Mathf.Sign(Random.Range(-1, 1)), Random.Range(0.1f, sway_Y) * Mathf.Sign(Random.Range(-1, 1)), thisPosition.z);
    }
}
