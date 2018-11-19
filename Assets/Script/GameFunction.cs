using UnityEngine;
using System.Collections;

public class GameFunction{

    //在指定物体下寻找目标名字的物体
	static public GameObject GetGameObjectInChildrenByName(GameObject obj,string name)
    {
        try
        {
            foreach (Transform t in obj.GetComponent<Transform>())
            {
                if (t.name == name)
                {
                    return t.gameObject;
                }
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    //贝塞尔曲线
    static public Vector3 BezierLine(Vector3 from, Vector3 to, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 t1, t2, t3, t4, t5;
        t1 = Vector3.Lerp(from, p1, t);
        t2 = Vector3.Lerp(p1, p2, t);
        t3 = Vector3.Lerp(p2, to, t);

        t4 = Vector3.Lerp(t1, t2, t);
        t5 = Vector3.Lerp(t2, t3, t);

        return Vector3.Lerp(t4, t5, t);
    }
}
