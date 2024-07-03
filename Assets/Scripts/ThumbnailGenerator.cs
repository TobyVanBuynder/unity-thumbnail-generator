using UnityEngine;
using GLTFast;

public class ThumbnailGenerator : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] Light _directionalLight;
    [SerializeField] Transform _holder;

    Vector3 _thumbnailObjectForward = -Vector3.forward;
    Vector3[] _boundsVertices = null;
    RenderTexture _cachedRenderTexture = null;
    int _renderTextureSize = 256;
    int _thumbnailLayerMask;

    public void Awake()
    {
        _thumbnailLayerMask = LayerMask.NameToLayer("Thumbnail");
        Disable();
    }

    public void Enable()
    {
        _camera.enabled = true;
        _directionalLight.enabled = true;
    }

    public void Disable()
    {
        _camera.enabled = false;
        _directionalLight.enabled = false;
    }

    public int GetThumbnailLayer()
    {
        return _thumbnailLayerMask;
    }

    public (GameObject, GltfAsset) CreateSetupPrefab()
    {
        GameObject go = new GameObject();
        go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        GltfAsset gltfAsset = go.AddComponent<GltfAsset>();
        gltfAsset.LoadOnStartup = false;
        gltfAsset.InstantiationSettings = new InstantiationSettings{
            Layer = GetThumbnailLayer(),
            SceneObjectCreation = SceneObjectCreation.Never
        };

        return (go, gltfAsset);
    }

    public RenderTexture GenerateThumbnail(Transform objectTransform)
    {
        Enable();

        var root = objectTransform.GetChild(0);
        root.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        root.localScale = Vector3.one;

        Transform oldParent = objectTransform.parent;
        Vector3 oldForward = objectTransform.forward;

        objectTransform.SetParent(_holder, false);
        objectTransform.forward = _thumbnailObjectForward;

        SetupCamera();
        Utils.FitObjectInOrthographic(objectTransform, _camera, ref _boundsVertices);

        RenderTexture renderTexture;
        GenerateRenderTexture(out renderTexture);

        objectTransform.forward = oldForward;
        objectTransform.SetParent(oldParent, false);

        Disable();

        return renderTexture;
    }

    void SetupCamera()
    {
        _camera.transform.position = -_camera.transform.forward * 10;
        _camera.aspect = 1;
        _camera.orthographicSize = 1;
    }

    void GenerateRenderTexture(out RenderTexture renderTexture)
    {
        if (!_cachedRenderTexture)
            _cachedRenderTexture = new RenderTexture(_renderTextureSize, _renderTextureSize, 0, RenderTextureFormat.ARGB32, 0);
        
        if (!_cachedRenderTexture.IsCreated())
        {
            _cachedRenderTexture.Create();
            _cachedRenderTexture.filterMode = FilterMode.Point;
        }
        else
        {
            Utils.ClearRenderTexture(_cachedRenderTexture);
        }

        if (_cachedRenderTexture.IsCreated())
        {
            _camera.targetTexture = _cachedRenderTexture;
            _camera.Render();
            renderTexture = _cachedRenderTexture;
            _camera.targetTexture = null;
        }
        else
        {
            _cachedRenderTexture = null;
            renderTexture = null;
        }
    }
}
