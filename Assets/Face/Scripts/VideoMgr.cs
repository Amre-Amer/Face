using UnityEngine;
using UnityEngine.Video;

public class VideoMgr : MonoBehaviour
{
    public GlobalsMgr g;
    void Awake()
    {
        g.toolsMgr.SetMosaicBlockSize((int)g.videoClip.width, (int)g.videoClip.height);
        g.videoPlayer.isLooping = true;
        g.videoPlayer.playOnAwake = false;
        g.videoPlayer.clip = g.videoClip;
        g.videoPlayer.sendFrameReadyEvents = true;
        g.videoPlayer.frameReady += OnNewFrameReady;
        
        // Ensure the VideoPlayer renders to a RenderTexture or APIOnly
        g.videoPlayer.renderMode = VideoRenderMode.APIOnly;
    }

    public void StartVideo()
    {
        g.videoPlayer.Play();
    }

    private void OnNewFrameReady(VideoPlayer source, long frameIdx)
    {
        RenderTexture renderTexture = source.texture as RenderTexture;
        if (renderTexture == null) return;

        // Initialize or resize the Texture2D to match the video dimensions
        if (g.currentFrameTexture == null || 
            g.currentFrameTexture.width != renderTexture.width || 
            g.currentFrameTexture.height != renderTexture.height)
        {
            g.currentFrameTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        }

        // Save the active RenderTexture, read pixels from the video, and restore
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture.active = renderTexture;

        g.currentFrameTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        g.currentFrameTexture.Apply();
        g.matDisplay.mainTexture = g.currentFrameTexture;

        RenderTexture.active = previousActive;
    }

    void OnDestroy()
    {
        g.videoPlayer.frameReady -= OnNewFrameReady;
    }
}
