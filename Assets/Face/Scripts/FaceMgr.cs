using UnityEngine;

public class FaceMgr : MonoBehaviour
{
    public GlobalsMgr g;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (g.ynWebcam)
        {
            g.webcamMgr.StartWebcam();
        } else
        {
            g.videoMgr.StartVideo();
        }
        InvokeRepeating(nameof(Fps), 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        g.matDisplay.mainTexture = g.currentFrameTexture;
        g.inferenceMgr.Infer();
        g.cntFps++;
    }

    void Fps()
    {
        g.textFps.text = "fps: " + g.cntFps;
        g.cntFps = 0;
    }
}
