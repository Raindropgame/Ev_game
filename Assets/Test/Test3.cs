using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class Test3 : MonoBehaviour
{
    public Transform a;

    private MeshFilter MF;


    private void Start()
    {
        MF = this.GetComponent<MeshFilter>();
    }

    private void OnEnable()
    {

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        //设置顶点  
        mesh.vertices = new Vector3[] { new Vector3(0, 3, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0) ,new Vector3(1,3,0)};
        //设置三角形顶点顺序，顺时针设置  
        mesh.triangles = new int[] {
            2,1,0,
            0,3,2,
        };
    }

    private void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        //设置顶点  
        mesh.vertices = new Vector3[] { new Vector3(0, 3, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), a.localPosition };
        //设置三角形顶点顺序，顺时针设置  
        mesh.triangles = new int[] {
            2,1,0,
            0,3,2,
        };

        Vector2[] uv = new Vector2[mesh.vertices.Length];
        for(int i = 0;i<uv.Length;i++)
        {
            uv[i] = mesh.vertices[i];
        }

        mesh.uv = uv;
    }


}
