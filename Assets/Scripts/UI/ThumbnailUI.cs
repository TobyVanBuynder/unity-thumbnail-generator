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

    void Start()
    {
        SetModelLoadedView(false);
    }

    void OnEnable()
    {
        GlobalEvents.OnModelLoaded += OnModelLoaded;
    }

    void OnDisable()
    {
        GlobalEvents.OnModelLoaded -= OnModelLoaded;
    }

    private void OnModelLoaded(string fileName, GameObject modelObject, Model.Type modelType)
    {
        throw new NotImplementedException();
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

    private void OnClickSelectFile()
    {
        ExtensionFilter[] fileTypes = new []{
            FileSystem.CreateFileType("GLTF scene", "gltf", "glb")
        };
        FileSystem.OpenPanelAsync("Import GLTF scene", fileTypes, Application.persistentDataPath, OnOpenFilePanelAsync);
    }

    private void OnClickExport()
    {
        ExtensionFilter[] fileTypes = new []{
            FileSystem.CreateFileType("Image (PNG)", "png")
        };
        FileSystem.SavePanelAsync("Export model file", "", fileTypes, Application.persistentDataPath, OnSaveFilePanelAsync);
    }

    private async void OnOpenFilePanelAsync(string[] fileNames)
    {
        if (fileNames.Length > 0)
        {
            string fileToLoad = fileNames[0];
            fileToLoad = Utils.FixFilePath(fileToLoad);
            
            ThumbnailGenerator generator = FindObjectOfType<ThumbnailGenerator>();
            (GameObject, GltfAsset) gltfTuple = generator.CreateSetupPrefab();
            if (await gltfTuple.Item2.Load(fileToLoad))
            {
                RenderTexture thumbnail = generator.GenerateThumbnail(gltfTuple.Item1.transform);
                _fileNameLabel.text = Path.GetFileNameWithoutExtension(fileToLoad);
                _thumbnailImage.image = thumbnail;
            }
            Destroy(gltfTuple.Item1);
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