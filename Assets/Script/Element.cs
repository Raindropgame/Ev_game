using UnityEngine;
using System.Collections;

public class Element : MonoBehaviour {

    //检测不连续

    public bool isAddRig = false;
    public Attribute element;
    [HideInInspector]
    public bool isTrigger = false;
    [HideInInspector]
    public ArrayList TriggerElement = new ArrayList();
    [HideInInspector]
    public bool isLock = false;

    private void Start()
    {
        if(isAddRig)
        {
            Rigidbody2D t;
            t = GetComponent<Rigidbody2D>();
            if (t == null)
            {
                t = this.gameObject.AddComponent<Rigidbody2D>();
                t.gravityScale = 0;
            }
            else
            {
                t.gravityScale = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        if (TriggerElement.Count > 0)
        {
            TriggerElement.Clear();
            isTrigger = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!isLock)
        {
            isTrigger = true;
            TriggerElement.Add(collision.GetComponent<Element>().element);
        }
    }


    //是否含有该元素
    public bool isContainElement(Attribute e)
    {
        if (TriggerElement.Count > 0)
        {
            if (TriggerElement.Contains(e))
            {
                return true;
            }
        }
        return false;
    }

}
