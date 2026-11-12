using UnityEngine;

public class WebcamMgr : MonoBehaviour
{
    public GlobalsMgr g;

    public void StartWebcam()
    {
        g.webCamTexture = new WebCamTexture();
        g.webCamTexture.Play();
        g.toolsMgr.SetMosaicBlockSize(g.webCamTexture.width, g.webCamTexture.height);
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
        Texture2D tex = new Texture2D(g.webCamTexture.width, g.webCamTexture.height, TextureFormat.RGBA32, false);
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
