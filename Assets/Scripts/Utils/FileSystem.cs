using SFB;
using System;
using System.Collections.Generic;
using System.Linq;

public static class FileSystem
{
    public class FileTypeArrayBuilder
    {
        List<ExtensionFilter> _fileTypes;

        public FileTypeArrayBuilder()
        {
            _fileTypes = new List<ExtensionFilter>();
        }

        public FileTypeArrayBuilder Add(string fileTypeName, params string[] fileTypes)
        {
            _fileTypes.Add(new ExtensionFilter(fileTypeName, fileTypes));
            return this;
        }

        public ExtensionFilter[] Build()
        {
            return _fileTypes.ToArray();
        }
    }

    public class FileTypeBuilder
    {
        string _fileTypeName;
        List<string> _fileTypes;

        public FileTypeBuilder(string fileTypeName)
        {
            _fileTypeName = fileTypeName;
            _fileTypes = new List<string>();
        }

        public FileTypeBuilder Add(string fileType)
        {
            _fileTypes.Add(fileType);
            return this;
        }

        public FileTypeBuilder Add(params string[] fileTypes)
        {
            _fileTypes.AddRange(fileTypes);
            return this;
        }

        public ExtensionFilter[] Build()
        {
            return new ExtensionFilter[]{ CreateFileType(_fileTypeName, _fileTypes) };
        }
    }

    public static ExtensionFilter CreateFileType(string fileTypeName, params string[] extensions)
    {
        return new ExtensionFilter(fileTypeName, extensions);
    }

    public static ExtensionFilter CreateFileType(string fileTypeName, ICollection<string> extensions)
    {
        return new ExtensionFilter(fileTypeName, extensions.ToArray());
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
