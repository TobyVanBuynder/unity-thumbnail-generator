using System.Threading.Tasks;
using UnityEngine;

public class ThumbnailFromModel : MonoBehaviour
{
    [SerializeField] ThumbnailGeneratorLoader _generatorLoader;
    [SerializeField] private int _renderTextureSize = 256;
    [SerializeField] private FilterMode _renderTextureFilterMode = FilterMode.Point;

    private readonly RenderTextureFormat _renderTextureFormat = RenderTextureFormat.ARGB32;
    private readonly int _renderTextureDepthBufferBits = 8;

    RenderTexture _thumbnail = null;

    void Awake()
    {
        if (_generatorLoader == null)
        {
            enabled = false;
        }
    }

    void OnEnable()
    {
        GlobalEvents.OnModelLoaded += OnModelLoaded;
    }

    void OnDisable()
    {
        GlobalEvents.OnModelLoaded -= OnModelLoaded;
    }

    private async void OnModelLoaded(string filePath, GameObject modelObject, Model.Type type)
    {
        if (IsOfTypeGLTF(type))
        {
            FixLoadedGltfTransform(modelObject);
        }

        if (_thumbnail == null)
        {
            CreateRenderTexture();
        }

        _generatorLoader.Get().GenerateThumbnail(modelObject.transform, _thumbnail);

        await Task.Yield();

        GlobalEvents.OnThumbnailLoaded?.Invoke(_thumbnail);
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
        _thumbnail = new RenderTexture(desc);
        _thumbnail.Create();
        _thumbnail.filterMode = _renderTextureFilterMode;
    }
}
