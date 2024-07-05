using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

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

        _selectFileButton = root.Q<Button>("SelectFileButton");
        
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
        GlobalEvents.OnSelectFile?.Invoke(Application.streamingAssetsPath);
    }

    private void OnClickExport()
    {
        GlobalEvents.OnExportFile?.Invoke(Application.streamingAssetsPath, _thumbnailImage.image, (Thumbnail.ExportMode)_exportFileTypeEnum.value);
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
        SetModelLoadedView(true);

        _fileNameLabel.text = Path.GetFileName(filePath);
    }

    private void OnThumbnailLoaded(Texture thumbnail)
    {
        _thumbnailImage.image = thumbnail;
    }
}