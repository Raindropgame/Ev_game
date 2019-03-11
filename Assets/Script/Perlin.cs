using UnityEngine;
using System.Collections;

public class Perlin{

    private int Size;
    public Vector2[][] Gradient_2D;

    public Perlin(int size)
    {
        this.Size = size;
        IntiGradient();
    }

    void IntiGradient()
    {
        Gradient_2D = new Vector2[Size][];
        for(int i = 0;i<Gradient_2D.Length;i++)
        {
            Gradient_2D[i] = new Vector2[Size];
            for(int j = 0;j<Size;j++)
            {
                Gradient_2D[i][j] = Random.insideUnitCircle.normalized;
            }
        }
    }

    float fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    public float getValue(Vector2 p)
    {
        int x_i = Mathf.FloorToInt(p.x);
        int y_i = Mathf.FloorToInt(p.y);
        float x_f = fade(p.x - x_i);
        float y_f = fade(p.y - y_i);
        Vector2 P = new Vector2(p.x - x_i, p.y - y_i);

        float G1 = Vector2.Dot(P - Vector2.zero, Gradient_2D[y_i % Size][x_i % Size]);
        float G2 = Vector2.Dot(P - Vector2.right, Gradient_2D[y_i % Size][(x_i + 1) % Size]);
        float G3 = Vector2.Dot(P - Vector2.up, Gradient_2D[(y_i + 1) % Size][x_i % Size]);
        float G4 = Vector2.Dot(P - Vector2.one, Gradient_2D[(y_i + 1) % Size][(x_i + 1) % Size]);

        return Mathf.Lerp(Mathf.Lerp(G1, G2, x_f), Mathf.Lerp(G3, G4, x_f), y_f);
    }   
}
