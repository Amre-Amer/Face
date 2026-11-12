using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class GlobalsMgr : MonoBehaviour
{
    [Header("Classes")]
    public WebcamMgr webcamMgr;
    public VideoMgr videoMgr;
    public ToolsMgr toolsMgr;
    
    [Header("Webcam or Video")]
    public bool ynWebcam;
    
    [Header("Inference")]
    public int width = 640;
    public int height = 640;
    
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
