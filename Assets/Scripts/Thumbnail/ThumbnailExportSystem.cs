using UnityEngine;

using FileTypeDictionary = System.Collections.Generic.Dictionary<Thumbnail.ExportMode, SFB.ExtensionFilter[]>;
using ExporterDictionary = System.Collections.Generic.Dictionary<Thumbnail.ExportMode, IThumbnailExporter>;

public class ThumbnailExportSystem : MonoBehaviour
{
    FileTypeDictionary _fileTypeDictionary;
    ExporterDictionary _exporterDictionary;

    Texture _textureToSave = null;
    Thumbnail.ExportMode _exportMode;

    void Awake()
    {
        _fileTypeDictionary = new FileTypeDictionary();
        _exporterDictionary = new ExporterDictionary();
    }

    void Start()
    {
        RegisterExportTypes();
    }

    private void RegisterExportTypes()
    {
        AddValidType(Thumbnail.ExportMode.PNG, new PNGThumbnailExporter(), "PNG image", "png");
        AddValidType(Thumbnail.ExportMode.JPG, new JPGThumbnailExporter(), "JPEG image", "jpg");
    }

    private void AddValidType(Thumbnail.ExportMode exportMode, IThumbnailExporter thumbnailExporter,string fileTypeName, string fileType)
    {
        _fileTypeDictionary.Add(exportMode, new []{FileSystem.CreateFileType(fileTypeName, fileType)});
        _exporterDictionary.Add(exportMode, thumbnailExporter);
    }

    void OnEnable()
    {
        GlobalEvents.OnExportFile += OnExportFile;
    }

    void OnDisable()
    {
        GlobalEvents.OnExportFile -= OnExportFile;
    }

    private void OnExportFile(string path, Texture texture, Thumbnail.ExportMode exportMode)
    {
        if (IsExportModeValid(exportMode))
        {
            _textureToSave = texture;
            _exportMode = exportMode;

            FileSystem.SavePanelAsync("Save thumbnail image", $"Thumbnail.{exportMode.ToString().ToLower()}", _fileTypeDictionary[exportMode], path, OnSaveThumbnail);
        }
        else
        {
            Debug.LogWarning($"ExportMode {exportMode} is invalid!");
        }
    }

    private void OnSaveThumbnail(string filePath)
    {
        if (filePath != null && filePath.Length > 0)
        {
            filePath = Utils.FixFilePath(filePath);

            IThumbnailExporter exporter = GetThumbnailExporter(_exportMode);
            exporter.Export(filePath, _textureToSave);
        }
    }

    private IThumbnailExporter GetThumbnailExporter(Thumbnail.ExportMode exportMode)
        => IsExportModeValid(exportMode) ? _exporterDictionary[exportMode] : null;

    private bool IsExportModeValid(Thumbnail.ExportMode exportMode)
        => _exporterDictionary.ContainsKey(exportMode);
}