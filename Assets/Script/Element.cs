using UnityEngine;
using System.Collections;

public class Element : MonoBehaviour {

    public Attribute element;
    [HideInInspector]
    public ArrayList TriggerElement = new ArrayList();

    private BoxCollider2D BoxColl;

    private void Start()
    {
        BoxColl = GetComponent<BoxCollider2D>();
    }


    //是否含有该元素
    public bool isContainElement(Attribute e)
    {
        getElements();
        if (TriggerElement.Count > 0)
        {
            if (TriggerElement.Contains(e))
            {
                return true;
            }           
        }
        return false;
    }

    void getElements()
    {
        TriggerElement.Clear();
        int mask = 1 << 16;
        RaycastHit2D[] hitPoints = Physics2D.BoxCastAll(BoxColl.bounds.center, GameFunction.getVector3(BoxColl.size.x * Mathf.Abs(transform.lossyScale.x), BoxColl.size.y * Mathf.Abs(transform.lossyScale.y), 1), transform.rotation.eulerAngles.z, Vector2.zero, 0, mask);
        for(int i = 0;i<hitPoints.Length;i++)
        {
            if (hitPoints[i].collider != BoxColl)  //排除自己
            {
                TriggerElement.Add(hitPoints[i].collider.GetComponent<Element>().element);
            }
        }
    }

}
