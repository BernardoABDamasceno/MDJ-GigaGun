using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRT : MonoBehaviour
{
    [SerializeField] RenderTexture lowResTexture;
    [SerializeField] Shader crtShader;

    [Range(1.0f, 100.0f)]
    public float curvature = 1.0f;

    [Range(1.0f, 100.0f)]
    public float vignetteWidth = 30.0f;

    [Range(1.0f, 100.0f)]
    public float lineSpeed = 1.0f;

    private Material crtMat;

    void Start()
    {
        crtMat ??= new Material(crtShader);
        crtMat.hideFlags = HideFlags.HideAndDontSave;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        crtMat.SetFloat("_LineSpeed", lineSpeed);
        crtMat.SetFloat("_Curvature", curvature);
        crtMat.SetFloat("_VignetteWidth", vignetteWidth);

        Graphics.Blit(source, lowResTexture);
        Graphics.Blit(lowResTexture, destination, crtMat);
    }
}