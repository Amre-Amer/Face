using TMPro;
using Unity.InferenceEngine;
using UnityEngine;
using UnityEngine.Video;

public class GlobalsMgr : MonoBehaviour
{
    [Header("Classes")]
    public WebcamMgr webcamMgr;
    public VideoMgr videoMgr;
    public InferenceMgr inferenceMgr;
    public ToolsMgr toolsMgr;
    
    [Header("Webcam or Video")]
    public bool ynWebcam;
    
    [Header("Inference")]
    public int width = 640;
    public int height = 640;
    public ModelAsset modelAsset;
    [Range(0.1f, 0.95f)] public float confidenceThreshold = 0.45f;
    [Range(0.1f, 0.95f)] public float iouThreshold = 0.45f;
    [Range(4f, 64f)] public float mosaicBlockSize = 24f;
    
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public VideoClip videoClip;
    
    [Header("Display")]
    public Material matDisplay;
    public TMP_Text textFps;
    
    [Header("Other")]
    public int cntFps;
    
    [Header("None at startup")]
    public Texture2D currentFrameTexture;
    public WebCamTexture webCamTexture;
}
