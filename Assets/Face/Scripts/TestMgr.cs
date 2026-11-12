using System.Collections.Generic;
using UnityEngine;

public class TestMgr : MonoBehaviour
{
    public GlobalsMgr g;
    public GameObject quad;
    private Material _mosaicMaterial;
    private const int MaxFaces = 20;
    private readonly Vector4[] _shaderBoxes = new Vector4[MaxFaces];
    [Range(4f, 64f)] float mosaicBlockSize = 24f;
    public Texture2D texOrig;
    Texture2D tex;
    float xMin = .25f;
    float yMin = .25f;
    float w = .5f;
    float h = .5f;
    float ang;

    void Start()
    {
        tex = g.toolsMgr.ResizeTexture(texOrig, 640, 640);
        quad.GetComponent<Renderer>().material.mainTexture = tex;
        _mosaicMaterial = quad.GetComponent<Renderer>().material;
        InvokeRepeating(nameof(Test), 1, .1f);
    }

    void Test()
    {
        xMin = .25f + .25f * Mathf.Cos(ang * Mathf.Deg2Rad);
        yMin = .25f + .25f * Mathf.Sin(ang * Mathf.Deg2Rad);
        List<FaceBox> faces = new();
        FaceBox face = new FaceBox
        {
            rect = new Rect(xMin, yMin, w, h),
            score = 0
        };
        faces.Add(face);
        UpdateShaderBoxes(faces);
        ang += 5;
    }

    private void UpdateShaderBoxes(List<FaceBox> faces)
    {
        int count = Mathf.Min(faces.Count, MaxFaces);
        for (int i = 0; i < count; i++)
        {
            Rect r = faces[i].rect;
            _shaderBoxes[i] = new Vector4(r.xMin, r.yMin, r.xMax, r.yMax);
        }

        _mosaicMaterial.SetFloat("_BlockSize", mosaicBlockSize);
        _mosaicMaterial.SetInt("_FaceCount", count);
        _mosaicMaterial.SetVectorArray("_FaceBoxes", _shaderBoxes);
    }
}
