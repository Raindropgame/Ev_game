using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class NoiseTexture : EditorWindow{

    [MenuItem("MyTools/Noise")]
    static void Window()
    {
        NoiseTexture window = (NoiseTexture)EditorWindow.GetWindow(typeof(NoiseTexture), false, "Noise");
        window.Show();
    }

    /// <summary>
    /// 纹理类型
    /// </summary>
    enum textureType
    {
        Normal,
        Type2,
        Type3,
        Type4,
        Type5
    };

    int Size = 0;
    int gridSize = 0;
    string textureName = null;
    textureType type = textureType.Normal;

    delegate float delegate_noise(Vector2 p, Perlin perlin);
    delegate_noise noise;
    Vector2 t_vec2 = Vector2.zero;

    private void OnGUI()
    {
        EditorGUILayout.LabelField("纹理大小");
        Size = EditorGUILayout.IntField(Size);
        EditorGUILayout.LabelField("噪声持续度");
        gridSize = EditorGUILayout.IntField(gridSize);
        type = (textureType)EditorGUILayout.EnumPopup("纹理类型",type);
        switch(type)
        {
            case textureType.Normal:
                noise = noise_normal;
                break;
            case textureType.Type2:
                noise = noise_type2;
                break;
            case textureType.Type3:
                noise = noise_type3;
                break;
            case textureType.Type4:
                noise = noise_type4;
                break;
            case textureType.Type5:
                noise = noise_type5;
                break;
            default:
                noise = noise_normal;
                break;
        }
        textureName = EditorGUILayout.TextField("纹理名称：", textureName);

        if(GUILayout.Button("生成", GUILayout.Width(200), GUILayout.Height(50)))
        {
            GenTexture();
        }
    }

    /// <summary>
    /// 生成纹理
    /// </summary>
    void GenTexture()
    {
        if (Size == 0 || gridSize == 0)
            return;

        Perlin perlin = new Perlin(gridSize);
        Texture2D texture = new Texture2D(Size,Size);
        float a = Size / gridSize;

        for(int i = 0;i<Size;i++)
        {
            for(int j = 0;j<Size;j++)
            {
                float t = noise(GameFunction.getVector2(i / a, j / a), perlin);
                t = 0.5f * t + 0.5f;
                texture.SetPixel(i, j, GameFunction.getColor(t, t, t, 1));
            }
        }
        texture.Apply();

        FileStream fs = new FileStream(Application.dataPath + "/" + textureName + ".jpg", FileMode.Create);
        byte[] data = texture.EncodeToJPG();
        fs.Write(data, 0, data.Length);
        fs.Flush();
        fs.Close();

        Debug.Log("生成成功");
    }

    float noise_normal(Vector2 p,Perlin perlin)
    {
        return perlin.getValue(p);
    }

    float noise_type2(Vector2 p,Perlin perlin)
    {
        return perlin.getValue(p) + 0.5f * perlin.getValue(2 * p) + 0.25f * perlin.getValue(4 * p) + 0.125f * perlin.getValue(8 * p);
    }

    float noise_type3(Vector2 p,Perlin perlin)
    {
        return Mathf.Abs(perlin.getValue(p) + 0.5f * perlin.getValue(2 * p) + 0.25f * perlin.getValue(4 * p) + 0.125f * perlin.getValue(8 * p));
    }

    float noise_type4(Vector2 p,Perlin perlin)
    {
        return Mathf.Sin(p.y + Mathf.Abs(perlin.getValue(p) + 0.5f * perlin.getValue(2 * p) + 0.25f * perlin.getValue(4 * p) + 0.125f * perlin.getValue(8 * p)));
    }

    float noise_type5(Vector2 p,Perlin perlin)
    {
        float t = perlin.getValue(p) * 20;
        return t - (int)t;
    }
}
