using UnityEngine;

public class WebcamMgr : MonoBehaviour
{
    public GlobalsMgr g;

    public void StartWebcam()
    {
        g.webCamTexture = new WebCamTexture();
        g.webCamTexture.Play();
    }

    void Update()
    {
        if (g.ynWebcam == false) return;
        CaptureFrame();
    }

    public void CaptureFrame()
    {
        // Ensure the webcam has started and has valid dimensions
        if (g.webCamTexture == null || !g.webCamTexture.isPlaying || g.webCamTexture.width < 100)
        {
            return;
        }

        // Create the Texture2D once, or recreate if resolution changes
        // if (snapTexture == null || snapTexture.width != webCamTexture.width || snapTexture.height != webCamTexture.height)
        // {
        Texture2D tex = new Texture2D(g.webCamTexture.width, g.webCamTexture.height, TextureFormat.RGBA32, false);
        // }

        // Copy pixel data and upload to the GPU
        tex.SetPixels32(g.webCamTexture.GetPixels32());
        tex.Apply();
        g.currentFrameTexture = tex;
    }

    void OnDestroy()
    {
        if (g.webCamTexture != null && g.webCamTexture.isPlaying)
        {
            g.webCamTexture.Stop();
        }
    }
}
