using UnityEngine;
using System.Collections;

public class ScreenBlur : PostEffectsBase
{


    public Shader BlurShader;
    private Material BlurMaterial = null;
    public Material blurMaterial
    {
        get
        {
            BlurMaterial = CheckShaderAndCreateMaterial(BlurShader, BlurMaterial);
            return BlurMaterial;
        }
    }

    public Shader CameraBlurShader;
    private Material CameraBlurMaterial = null;
    public Material cameraBlurMaterial
    {
        get
        {
            CameraBlurMaterial = CheckShaderAndCreateMaterial(CameraBlurShader, CameraBlurMaterial);
            return CameraBlurMaterial;
        }
    }

    [Range(1, 20)]
    public int downSample = 1;
    [Range(0, 1)]
    public float focus = 0.5f;

    public Color BGColor;
    
    private void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (blurMaterial != null && cameraBlurMaterial != null)
        {
            RenderTexture tTex = RenderTexture.GetTemporary(src.width / downSample, src.height / downSample, 0);
            RenderTexture BlurTex = RenderTexture.GetTemporary(src.width / downSample, src.height / downSample, 0);
            RenderTexture DepthTex = RenderTexture.GetTemporary(src.width, src.height, 0);
            tTex.filterMode = FilterMode.Bilinear;
            BlurTex.filterMode = FilterMode.Bilinear;
            Graphics.Blit(src, tTex, blurMaterial,0);
            Graphics.Blit(tTex, BlurTex, blurMaterial, 1);
            cameraBlurMaterial.SetTexture("_BlurTex", BlurTex);
            cameraBlurMaterial.SetFloat("focus", focus);
            cameraBlurMaterial.SetColor("_Color", BGColor);
            Graphics.Blit(src, dest, cameraBlurMaterial);

            RenderTexture.ReleaseTemporary(tTex);//释放纹理内存
            RenderTexture.ReleaseTemporary(BlurTex);
            RenderTexture.ReleaseTemporary(DepthTex);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}