using System;
using UnityEngine;

public static class GlobalEvents
{
    public static event Action<string> OnOpenFile;
    public static event Action<string, Texture, Thumbnail.ExportMode> OnSaveThumbnail;
    public static event Action<string, GameObject, Model.Type> OnModelLoaded;
}