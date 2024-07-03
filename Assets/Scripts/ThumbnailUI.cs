using UnityEngine;
using UnityEngine.UIElements;
using SFB;
using System.IO;
using GLTFast;
using UnityEngine.Rendering;

public class ThumbnailUI : MonoBehaviour
{
    [SerializeField] UIDocument _uiDocument;
    
    Button _selectFileButton;

    VisualElement _propertiesBox;
    Label _boundsLabelX;
    Label _boundsLabelY;
    Label _boundsLabelZ;

    void Awake()
    {
        VisualElement root = _uiDocument.rootVisualElement;
        
        _selectFileButton = root.Q<Button>("SelectButton");
        
        _propertiesBox = root.Q<VisualElement>("Properties");
        _boundsLabelX = root.Q<Label>("BoundsX");
        _boundsLabelY = root.Q<Label>("BoundsY");
        _boundsLabelZ = root.Q<Label>("BoundsZ");

        _propertiesBox.style.display = DisplayStyle.None;
    }

    void Start() {}

    void OnEnable()
    {
        _selectFileButton.clicked += OnClickSelectFile;
    }

    void OnDisable()
    {
        _selectFileButton.clicked -= OnClickSelectFile;
    }

    void OnClickSelectFile()
    {
        ExtensionFilter[] fileTypes = new []{
            FileSystem.CreateFileType("GLTF scene", "gltf", "glb")
        };
        FileSystem.OpenPanelAsync("Import GLTF scene", fileTypes, Application.persistentDataPath, OnOpenFilePanelAsync);
    }

    void OnClickExport()
    {
        ExtensionFilter[] fileTypes = new []{
            FileSystem.CreateFileType("Model file", "model")
        };
        FileSystem.SavePanelAsync("Export model file", "", fileTypes, Application.persistentDataPath, OnSaveFilePanelAsync);
    }

    void OnSaveFilePanelAsync(string filePath)
    {
        if (filePath != null && filePath.Length > 0)
        {
            // Save thumbnail
            RenderTexture thumbnail = null;
            Texture2D tex2D = new Texture2D(thumbnail.width, thumbnail.height, TextureFormat.ARGB32, 1, true);
            AsyncGPUReadback.Request(thumbnail, 0, TextureFormat.ARGB32, (asyncAction) => {

                tex2D.LoadRawTextureData(asyncAction.GetData<byte>());
                tex2D.EncodeToPNG();
            });
        }
    }

    async void OnOpenFilePanelAsync(string[] fileNames)
    {
        if (fileNames.Length > 0)
        {
            string fileToLoad = fileNames[0].Replace("\\", "\\\\");
            
            ThumbnailGenerator generator = FindObjectOfType<ThumbnailGenerator>();
            (GameObject, GltfAsset) gltfTuple = generator.CreateSetupPrefab();
            if (await gltfTuple.Item2.Load(fileToLoad))
            {
                RenderTexture thumbnail = generator.GenerateThumbnail(gltfTuple.Item1.transform);
                Vector3 boundSize = Utils.GetTotalBounds(gltfTuple.Item1.transform).size;
                boundSize.x = Utils.CeilToNearestHalf(boundSize.x);
                boundSize.y = Utils.CeilToNearestHalf(boundSize.y);
                boundSize.z = Utils.CeilToNearestHalf(boundSize.z);

                _propertiesBox.style.display = DisplayStyle.Flex;

                _boundsLabelX.Q<Label>().text = Path.GetFileNameWithoutExtension(fileToLoad);
                _boundsLabelX.Q<Image>().image = thumbnail;
                _boundsLabelX.text = "X: " + boundSize.x;
                _boundsLabelY.text = "Y: " + boundSize.y;
                _boundsLabelZ.text = "Z: " + boundSize.z;
            }
            Destroy(gltfTuple.Item1);
        }
    }
}