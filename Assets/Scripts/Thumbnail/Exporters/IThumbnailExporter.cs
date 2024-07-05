using UnityEngine;

public interface IThumbnailExporter
{
    void Export(string filePath, Texture texture);
    int GetMipCount();
    TextureFormat GetTextureFormat();
}