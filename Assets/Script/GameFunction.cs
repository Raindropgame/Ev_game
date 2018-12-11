using UnityEngine;
using System.Collections;

public class GameFunction{

    //原色透明
    static public Color Transparent = new Color(1,1,1,0);

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

    //vector3
    static public Vector3 t_Vector3 = Vector3.zero;

    static public Vector3 getVector3(float x,float y,float z)
    {
        t_Vector3.Set(x, y, z);
        return t_Vector3;
    }

    private static Color m_Color = Color.white;
    static public Color getColor(float r,float g,float b,float a)
    {
        m_Color.r = r;
        m_Color.g = g;
        m_Color.b = b;
        m_Color.a = a;
        return m_Color;
    }

}
