using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class PNGThumbnailExporter : IThumbnailExporter
{
    public void Export(string filePath, Texture texture)
    {
        AsyncGPUReadback.Request(texture, 0, TextureFormat.ARGB32, (asyncAction) => {
            Texture2D tex2D = new Texture2D(texture.width, texture.height, GetTextureFormat(), GetMipCount(), true);
            tex2D.LoadRawTextureData(asyncAction.GetData<byte>());
            File.WriteAllBytes(filePath, tex2D.EncodeToPNG());
        });
    }

    public int GetMipCount()
    {
        return 1;
    }

    public TextureFormat GetTextureFormat()
    {
        return TextureFormat.ARGB32;
    }
}