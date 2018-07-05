using UnityEngine;
using UnityEditor;
using System.Collections;

public class createPlane : EditorWindow {

    [MenuItem("MyTools/CreatePlane")]
    static void Window()
    {
        createPlane window = (createPlane)EditorWindow.GetWindow(typeof(createPlane), false, "CreatePlane");
        window.Show();
    }

    GameObject _object;

    int num_width;  //长宽上的顶点数
    int num_height;

    float width;  //每个顶点之间的距离
    float height;

    private void OnGUI()
    {
        _object = EditorGUILayout.ObjectField("网格依附物体:",_object,typeof(GameObject),true) as GameObject;
        EditorGUILayout.LabelField("---长宽上的顶点数---");
        num_width = EditorGUILayout.IntField("横向定点数:", num_width);
        num_height = EditorGUILayout.IntField("纵向顶点数:", num_height);
        EditorGUILayout.LabelField("---每个顶点之间的距离---");
        width = EditorGUILayout.FloatField("每个顶点之间的宽度:", width);
        height = EditorGUILayout.FloatField("每个顶点之间的高度:", height);

        EditorGUILayout.Space();

        if(GUILayout.Button("生成",GUILayout.Width(200),GUILayout.Height(50)))
        {
            build();
        }
    }

    void build()   //构造网格
    {
        if(_object == null)
        {
            Debug.Log("No GameObject");
            return;
        }

        MeshFilter MeshFilter = _object.GetComponent<MeshFilter>();
        if(MeshFilter == null)
        {
            MeshFilter =  _object.AddComponent<MeshFilter>();
        }

        Mesh mesh = MeshFilter.mesh;

        Vector3[] vertex = new Vector3[num_height * num_width];
        int[] triangles = new int[(num_width - 1) * (num_height - 1) * 6];
        Vector2[] uv = new Vector2[num_height * num_width];

        Vector3 centerPos = Vector3.zero;
        for(int i = 0;i<num_height;i++)
        {
            for(int j = 0;j<num_width;j++)
            {
                vertex[i * num_width + j] = centerPos + new Vector3(j * width,  - i * height, 0);  //每个顶点的位置
                uv[i * num_width + j] = new Vector2(Mathf.Lerp(0, 1, j / (float)(num_width - 1)), Mathf.Lerp(1, 0, i / (float)(num_height - 1)));   //UV坐标
            }
        }

        //构造上三角
        int t = 0;
        for(int i = 0;i<num_height - 1;i++)
        {
            for(int j = 0;j<num_width - 1;j++)
            {
                triangles[t] = i * (num_width) + j;
                triangles[t + 1] = triangles[t] + 1;
                triangles[t + 2] = triangles[t + 1] + num_width;
                t = t + 3;
            }
        }
        //构造下三角
        for (int i = 0; i < num_height - 1; i++)
        {
            for (int j = 0; j < num_width - 1; j++)
            {
                triangles[t] = i * (num_width) + j;
                triangles[t + 1] = triangles[t] + num_width + 1;
                triangles[t + 2] = triangles[t + 1] - 1;
                t = t + 3;
            }
        }
        mesh.vertices = vertex;
        mesh.triangles = triangles;
        mesh.uv = uv;

        Debug.Log("OK");
    }
}
