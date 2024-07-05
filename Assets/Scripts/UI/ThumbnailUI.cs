using UnityEngine;
using UnityEngine.UIElements;
using SFB;
using System.IO;
using GLTFast;
using UnityEngine.Rendering;
using System;

public class ThumbnailUI : MonoBehaviour
{
    [SerializeField] UIDocument _uiDocument;

    VisualElement _openFileView;
    VisualElement _modelLoadedView;

    Button _selectFileButton;

    Image _thumbnailImage;
    Label _fileNameLabel;
    EnumField _exportFileTypeEnum;
    Button _exportButton;

    void Awake()
    {
        InitializeVariablesFromRoot(_uiDocument.rootVisualElement);
        HookButtons();
    }

    private void InitializeVariablesFromRoot(VisualElement root)
    {
        _openFileView = root.Q<VisualElement>("OpenFileView");
        _modelLoadedView = root.Q<VisualElement>("ModelLoadedView");

        _selectFileButton = root.Q<Button>("OpenFileButton");
        
        _thumbnailImage = root.Q<Image>("Thumbnail");
        _fileNameLabel = root.Q<Label>("FileName");
        _exportFileTypeEnum = root.Q<EnumField>("ExportFileType");
        _exportButton = root.Q<Button>("ExportButton");
    }

    private void HookButtons()
    {
        _selectFileButton.clicked += OnClickSelectFile;
        _exportButton.clicked += OnClickExport;
    }

    private void OnClickSelectFile()
    {
        GlobalEvents.OnSelectFile?.Invoke(Application.persistentDataPath);
    }

    private void OnClickExport()
    {
        // TODO: move to event call
        ExtensionFilter[] fileTypes = new []{
            FileSystem.CreateFileType("Image (PNG)", "png")
        };
        FileSystem.SavePanelAsync("Export model file", "", fileTypes, Application.persistentDataPath, OnSaveFilePanelAsync);
    }

    void Start()
    {
        SetModelLoadedView(false);
    }

    private void SetModelLoadedView(bool isEnabled)
    {
        if (isEnabled)
        {
            _openFileView.style.display = DisplayStyle.None;
            _modelLoadedView.style.display = DisplayStyle.Flex;
        }
        else
        {
            _openFileView.style.display = DisplayStyle.Flex;
            _modelLoadedView.style.display = DisplayStyle.None;
        }
    }

    void OnEnable()
    {
        GlobalEvents.OnModelLoaded += OnModelLoaded;
        GlobalEvents.OnThumbnailLoaded += OnThumbnailLoaded;
    }

    void OnDisable()
    {
        GlobalEvents.OnModelLoaded -= OnModelLoaded;
        GlobalEvents.OnThumbnailLoaded -= OnThumbnailLoaded;
    }

    private void OnModelLoaded(string filePath, GameObject _, Model.Type modelType)
    {
        _fileNameLabel.text = Path.GetFileName(filePath);
    }

    private void OnThumbnailLoaded(Texture thumbnail)
    {
        _thumbnailImage.image = thumbnail;
    }

    private async void OnOpenFilePanelAsync(string[] fileNames)
    {
        if (fileNames.Length > 0)
        {
            string fileToLoad = fileNames[0];
            fileToLoad = Utils.FixFilePath(fileToLoad);

            GltfAsset asset = new GltfAsset();
            
            if (await asset.Load(fileToLoad))
            {
                ThumbnailGenerator generator = FindObjectOfType<ThumbnailGenerator>();
                RenderTexture thumbnail = null;
                GameObject ob = new GameObject();
                generator.GenerateThumbnail(ob.transform, thumbnail);
                Destroy(ob);
            }
        }
    }

    private async void OnSaveFilePanelAsync(string filePath)
    {
        if (filePath != null && filePath.Length > 0)
        {
            // Save thumbnail
            Texture thumbnail = _thumbnailImage.image;
            Texture2D tex2D = new Texture2D(thumbnail.width, thumbnail.height, TextureFormat.ARGB32, 1, true);
            AsyncGPUReadback.Request(thumbnail, 0, TextureFormat.ARGB32, (asyncAction) => {

                tex2D.LoadRawTextureData(asyncAction.GetData<byte>());
                tex2D.EncodeToPNG();
            });
        }
    }
}