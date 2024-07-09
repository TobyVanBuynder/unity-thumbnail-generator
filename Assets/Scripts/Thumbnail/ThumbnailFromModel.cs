using System.Threading.Tasks;
using UnityEngine;

public class ThumbnailFromModel : MonoBehaviour
{
    [SerializeField] ThumbnailGeneratorLoader _generatorLoader;
    [SerializeField] private int _renderTextureSize = 256;
    [SerializeField] private FilterMode _renderTextureFilterMode = FilterMode.Point;
    [SerializeField] private float _rotationStep = 15f;

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

    private void OnThumbnailRotateLeft()
    {
        RotateThumbnailCamera(_rotationStep);
        RegenerateThumbnail();
    }

    private void OnThumbnailRotateRight()
    {
        RotateThumbnailCamera(-_rotationStep);
        RegenerateThumbnail();
    }

    private void RotateThumbnailCamera(float rotationAngle)
    {
        ThumbnailGenerator thumbnailGenerator = _generatorLoader.Get();
        ThumbnailCamera thumbnailCamera = thumbnailGenerator.GetThumbnailCamera();
        thumbnailCamera.RotateY(rotationAngle);
    }

    private async void RegenerateThumbnail()
    {
        if (_lastLoadedModel != null)
        {
            await RenderThumbnailFromModel(_lastLoadedModel, _lastLoadedModelType);
        }
    }

    private async void OnModelLoaded(string _, GameObject modelObject, Model.Type type)
    {
        await RenderThumbnailFromModel(modelObject, type);
    }

    private async Task RenderThumbnailFromModel(GameObject modelObject, Model.Type type)
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

        await Task.Yield();

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
