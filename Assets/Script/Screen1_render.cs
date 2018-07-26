using UnityEngine;
using System.Collections;

public class Screen1_render : PostEffectsBase
{
    //对相机1的渲染

    static public Screen1_render instance;

    private Camera _camera;

    public Shader CameraWaveShader;
    private Material CameraWaveMaterial = null;
    public Material cameraWaveMaterial
    {
        get
        {
            CameraWaveMaterial = CheckShaderAndCreateMaterial(CameraWaveShader, CameraWaveMaterial);
            return CameraWaveMaterial;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        _camera = this.GetComponent<Camera>();
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
    }

    
    public float offset = 1;
    private float pivot_x = 0.5f;
    private float pivot_y = 0.5f;
    public float scale;

    private bool isWave = false;

    //[ImageEffectOpaque]
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (cameraWaveMaterial != null && isWave)
        {
            cameraWaveMaterial.SetFloat("_Pivot_x", pivot_x);
            cameraWaveMaterial.SetFloat("_Pivot_y", pivot_y);
            cameraWaveMaterial.SetFloat("_Offset", offset);
            cameraWaveMaterial.SetFloat("_Scale", scale);
            Graphics.Blit(src, dest, cameraWaveMaterial, 0);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    public void Wave(Vector2 pos,float time)
    {
        if (!isWave)
        {
            Vector2 pivot = _camera.WorldToScreenPoint(pos);
            pivot_x = pivot.x / _camera.pixelWidth;
            pivot_y = pivot.y / _camera.pixelHeight;
            StartCoroutine(startWave(pivot, time));
        }
    }

    IEnumerator startWave(Vector2 pivot,float time)
    {
        isWave = true;
        float _time = 0;
        while(_time < time)
        {
            _time += Time.deltaTime;

            cameraWaveMaterial.SetFloat("_limit", Mathf.Lerp(0,2,_time / time));
            yield return null;
        }
        isWave = false;
    }
}