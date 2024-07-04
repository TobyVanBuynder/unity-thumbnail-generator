using SFB;
using System;

public static class FileSystem
{
    static class FileTypeFactory
    {
        public static ExtensionFilter Create(string fileTypeName, params string[] extensions)
        {
            return new ExtensionFilter(fileTypeName, extensions);
        }
    }

    public static ExtensionFilter CreateFileType(string fileTypeName, params string[] extensions)
    {
        return FileTypeFactory.Create(fileTypeName, extensions);
    }

    public static void OpenPanelAsync(string windowTitle, ExtensionFilter[] fileTypes, string path, Action<string[]> callback)
    {
        StandaloneFileBrowser.OpenFilePanelAsync(windowTitle, path, fileTypes, false, callback);
    }

    public static void SavePanelAsync(string windowTitle, string fileName, ExtensionFilter[] fileTypes, string path, Action<string> callback)
    {
        StandaloneFileBrowser.SaveFilePanelAsync(windowTitle, path, fileName, fileTypes, callback);
    }
}
