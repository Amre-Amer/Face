using System.Collections.Generic;
using UnityEngine;
using Unity.InferenceEngine; // Change to Unity.Sentis if using Sentis 2.1

public class InferenceMgr : MonoBehaviour
{
    public GlobalsMgr g;

    private const int ModelSize = 640;
    private const int MaxFaces = 20;

    private Worker _worker;
    private Material _mosaicMaterial;
    private readonly Vector4[] _shaderBoxes = new Vector4[MaxFaces];
    // Texture2D tex;

    void Start()
    {
        _mosaicMaterial = g.matDisplay;
        Model model = ModelLoader.Load(g.modelAsset);
        _worker = new Worker(model, BackendType.GPUCompute);
    }

    public void Infer()
    {
        if (g.currentFrameTexture == null) return;
        Texture activeTex = TextureUtility.ResizeTexture(g.currentFrameTexture, 640, 640);

        if (activeTex == null || activeTex.width <= 16) return;

        // Convert Texture to [1, 3, 640, 640] Tensor
        using Tensor<float> inputTensor = TextureConverter.ToTensor(
            activeTex, ModelSize, ModelSize, channels: 3
        );

        _worker.Schedule(inputTensor);

        // Read back output tensor [1, 5, 8400] for single-class YOLO face models
        using Tensor<float> output = (_worker.PeekOutput() as Tensor<float>).ReadbackAndClone();

        List<FaceBox> detections = ParseYoloOutput(output);
        List<FaceBox> filtered = NonMaxSuppression(detections, g.iouThreshold);

        UpdateShaderBoxes(filtered);
    }

    private List<FaceBox> ParseYoloOutput(Tensor<float> output)
    {
        var boxes = new List<FaceBox>();
        int numAnchors = output.shape[2]; // Typically 8400 for 640x640

        for (int i = 0; i < numAnchors; i++)
        {
            float score = output[0, 4, i];
            if (score < g.confidenceThreshold) continue;

            // YOLO outputs center-x, center-y, width, height in 0..640 space (top-left origin)
            float cx = output[0, 0, i] / ModelSize;
            float cy = output[0, 1, i] / ModelSize;
            float w  = output[0, 2, i] / ModelSize;
            float h  = output[0, 3, i] / ModelSize;

            // Convert top-left image coords to bottom-left Unity UV coords
            float xMin = Mathf.Clamp01(cx - w * 0.5f);
            float yMin = Mathf.Clamp01(1f - (cy + h * 0.5f));

            boxes.Add(new FaceBox
            {
                rect = new Rect(xMin, yMin, Mathf.Clamp01(w), Mathf.Clamp01(h)),
                score = score
            });
        }
        return boxes;
    }

    private List<FaceBox> NonMaxSuppression(List<FaceBox> boxes, float iouThresh)
    {
        boxes.Sort((a, b) => b.score.CompareTo(a.score));
        var kept = new List<FaceBox>();

        while (boxes.Count > 0 && kept.Count < MaxFaces)
        {
            FaceBox best = boxes[0];
            kept.Add(best);
            boxes.RemoveAt(0);

            for (int i = boxes.Count - 1; i >= 0; i--)
            {
                if (CalculateIoU(best.rect, boxes[i].rect) > iouThresh)
                    boxes.RemoveAt(i);
            }
        }
        return kept;
    }

    private float CalculateIoU(Rect a, Rect b)
    {
        float x1 = Mathf.Max(a.xMin, b.xMin);
        float y1 = Mathf.Max(a.yMin, b.yMin);
        float x2 = Mathf.Min(a.xMax, b.xMax);
        float y2 = Mathf.Min(a.yMax, b.yMax);

        float interArea = Mathf.Max(0f, x2 - x1) * Mathf.Max(0f, y2 - y1);
        float unionArea = a.width * a.height + b.width * b.height - interArea;
        return unionArea > 0f ? interArea / unionArea : 0f;
    }

    private void UpdateShaderBoxes(List<FaceBox> faces)
    {
        int count = Mathf.Min(faces.Count, MaxFaces);
        for (int i = 0; i < count; i++)
        {
            Rect r = faces[i].rect;
            _shaderBoxes[i] = new Vector4(r.xMin, r.yMin, r.xMax, r.yMax);
        }

        _mosaicMaterial.SetFloat("_BlockSize", g.mosaicBlockSize);
        _mosaicMaterial.SetInt("_FaceCount", count);
        _mosaicMaterial.SetVectorArray("_FaceBoxes", _shaderBoxes);
    }

    void OnDestroy()
    {
        _worker?.Dispose();
    }
}
public struct FaceBox
{
    public Rect rect; // Normalized 0..1 (xMin, yMin, width, height) in UV space
    public float score;
}
