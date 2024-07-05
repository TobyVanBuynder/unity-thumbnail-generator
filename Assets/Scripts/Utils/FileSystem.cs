using SFB;
using System;
using System.Collections.Generic;

public static class FileSystem
{
    static class FileTypeFactory
    {
        public static ExtensionFilter Create(string fileTypeName, params string[] extensions)
        {
            return new ExtensionFilter(fileTypeName, extensions);
        }
    }

    public class FileTypeBuilder
    {
        List<ExtensionFilter> _fileTypes;

        public FileTypeBuilder()
        {
            _fileTypes = new List<ExtensionFilter>();
        }

        public FileTypeBuilder Add(string fileTypeName, params string[] fileTypes)
        {
            _fileTypes.Add(new ExtensionFilter(fileTypeName, fileTypes));
            return this;
        }

        public ExtensionFilter[] Build()
        {
            return _fileTypes.ToArray();
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
