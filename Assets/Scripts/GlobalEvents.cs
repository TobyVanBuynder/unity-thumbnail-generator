using System;
using UnityEngine;

public static class GlobalEvents
{
    public static Action<string> OnSelectFile;
    public static Action<string, Texture, Thumbnail.ExportMode> OnSaveThumbnail;
    public static Action<string, GameObject, Model.Type> OnModelLoaded;
    public static Action<Texture> OnThumbnailLoaded;
}