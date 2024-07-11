using System.Threading.Tasks;
using UnityEngine;

public class ThumbnailFromModel : MonoBehaviour
{
    [SerializeField] ThumbnailGeneratorLoader _generatorLoader;
    [SerializeField] private int _renderTextureSize = 256;
    [SerializeField] private FilterMode _renderTextureFilterMode = FilterMode.Point;
    [SerializeField] private float _rotationStep = 15f;
    [SerializeField] private bool _autoHideModel = true;

    private readonly RenderTextureFormat _renderTextureFormat = RenderTextureFormat.ARGB32;
    private readonly int _renderTextureDepthBufferBits = 8;

    GameObject _lastLoadedModel;
    Model.Type _lastLoadedModelType;
    RenderTexture _lastRenderedThumbnail;

    void Awake()
    {
        if (_generatorLoader == null) enabled = false;
    }

    void OnEnable()
    {
        GlobalEvents.OnModelLoaded += OnModelLoaded;
        GlobalEvents.OnThumbnailRotateLeft += OnThumbnailRotateLeft;
        GlobalEvents.OnThumbnailRotateRight += OnThumbnailRotateRight;
    }

    void OnDisable()
    {
        GlobalEvents.OnModelLoaded -= OnModelLoaded;
        GlobalEvents.OnThumbnailRotateLeft -= OnThumbnailRotateLeft;
        GlobalEvents.OnThumbnailRotateRight -= OnThumbnailRotateRight;
    }

    void OnDestroy()
    {
        _lastRenderedThumbnail.Release();
    }

    private void OnThumbnailRotateLeft()
    {
        RotateThumbnail(_rotationStep);
        RegenerateThumbnail();
    }

    private void OnThumbnailRotateRight()
    {
        RotateThumbnail(-_rotationStep);
        RegenerateThumbnail();
    }

    private void RotateThumbnail(float rotationAngle)
    {
        ThumbnailGenerator thumbnailGenerator = _generatorLoader.Get();
        ThumbnailCamera thumbnailCamera = thumbnailGenerator.GetThumbnailCamera();
        thumbnailCamera.RotateY(rotationAngle);
        ThumbnailLight thumbnailLight = thumbnailGenerator.GetThumbnailLight();
        thumbnailLight.RotateY(rotationAngle);
    }

    private void RegenerateThumbnail()
    {
        if (_lastLoadedModel != null)
        {
            RenderThumbnailFromModel(_lastLoadedModel, _lastLoadedModelType);
        }
    }

    private void OnModelLoaded(string _, GameObject modelObject, Model.Type type)
    {
        RenderThumbnailFromModel(modelObject, type);
    }

    private void RenderThumbnailFromModel(GameObject modelObject, Model.Type type)
    {
        _lastLoadedModel = modelObject;
        _lastLoadedModelType = type;

        if (IsOfTypeGLTF(_lastLoadedModelType))
        {
            FixLoadedGltfTransform(_lastLoadedModel);
        }

        if (_lastRenderedThumbnail == null)
        {
            CreateRenderTexture();
        }

        ThumbnailGenerator thumbnailGenerator = _generatorLoader.Get();
        thumbnailGenerator.GenerateThumbnail(_lastLoadedModel.transform, _lastRenderedThumbnail);

        if (_autoHideModel)
        {
            _lastLoadedModel.SetActive(false);
        }

        GlobalEvents.OnThumbnailLoaded?.Invoke(_lastRenderedThumbnail);
    }

    private bool IsOfTypeGLTF(Model.Type type)
    {
        return type == Model.Type.GLTF || type == Model.Type.GLB;
    }

    private void FixLoadedGltfTransform(GameObject modelObject)
    {
        Transform modelTransform = modelObject.transform;
        modelTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        modelTransform.localScale = Vector3.one;
    }

    private void CreateRenderTexture()
    {
        RenderTextureDescriptor desc = new RenderTextureDescriptor(_renderTextureSize, _renderTextureSize, _renderTextureFormat, _renderTextureDepthBufferBits)
        {
            sRGB = true
        };
        _lastRenderedThumbnail = new RenderTexture(desc);
        _lastRenderedThumbnail.Create();
        _lastRenderedThumbnail.filterMode = _renderTextureFilterMode;
    }
}
