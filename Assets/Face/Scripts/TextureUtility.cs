using UnityEngine;

public static class TextureUtility
{
    public static Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        // Create a temporary RenderTexture of the target size
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Bilinear;

        // Set the active RenderTexture and blit the source texture onto it
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);

        // Create the new Texture2D and read the RenderTexture pixels into it
        Texture2D result = new Texture2D(newWidth, newHeight, source.format, false);
        result.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        result.Apply();

        // Clean up the temporary RenderTexture
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }
}
