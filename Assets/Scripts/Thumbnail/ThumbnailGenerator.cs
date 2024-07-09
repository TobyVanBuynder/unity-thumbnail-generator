using UnityEngine;

public class ThumbnailGenerator : MonoBehaviour
{

    [Header("Adjustable")]
    [SerializeField] private string _thumbnailLayerName = "Thumbnail";
    [SerializeField] private FilterMode _textureFilterMode = FilterMode.Point;

    [Header("Linked (DO NOT TOUCH)")]
    [SerializeField] private ThumbnailCamera _thumbnailCamera;
    [SerializeField] private ThumbnailLight _thumbnailLight;
    [SerializeField] private Transform _objectHolderTransform;

    private readonly int _defaultRTSize = 256;
    private readonly RenderTextureFormat _defaultRTFormat = RenderTextureFormat.ARGB32;
    private readonly int _defaultRTDepthBufferBits = 8;
    private readonly Vector3 _thumbnailObjectForward = -Vector3.forward;

    private int _thumbnailLayerMask;

    void Awake()
    {
        if (_thumbnailCamera == null || _thumbnailLight == null || _objectHolderTransform == null)
        {
            enabled = false;
            return;
        }

        _thumbnailLayerMask = Utils.GetLayerMaskFromName(_thumbnailLayerName);
    }

    void Start()
    {
        Utils.SetLayerMask(GetThumbnailLayer(), transform);
        Utils.SetLayerMask(GetThumbnailLayer(), _thumbnailCamera.transform);
        Utils.SetLayerMask(GetThumbnailLayer(), _thumbnailLight.transform);
        Disable();
    }

    private void Enable()
    {
        _thumbnailCamera.Enable();
        _thumbnailLight.Enable();
    }

    private void Disable()
    {
        _thumbnailCamera.Disable();
        _thumbnailLight.Disable();
    }

    public void GenerateThumbnail(Transform objectTransform, RenderTexture renderTexture = null)
    {
        if (objectTransform != null)
        {
            Transform oldParent = objectTransform.parent;
            Vector3 oldForward = objectTransform.forward;
            LayerMask oldLayerMask = objectTransform.gameObject.layer;

            Enable();

            SetObjectProperties(objectTransform, _objectHolderTransform, _thumbnailObjectForward, GetThumbnailLayer());

            SetupCamera(objectTransform);
            GenerateTexture(renderTexture);

            SetObjectProperties(objectTransform, oldParent, oldForward, oldLayerMask);

            Disable();
        }
    }

    private void SetObjectProperties(Transform objectTransform, Transform parent, Vector3 forward, LayerMask layerMask)
    {
        objectTransform.SetParent(parent, false);
        objectTransform.forward = forward;
        Utils.SetLayerMask(layerMask, objectTransform, true);
    }

    private void SetupCamera(Transform objectTransform)
    {
        _thumbnailCamera.Reset();
        _thumbnailCamera.FitObjectInCamera(objectTransform);
    }

    private void GenerateTexture(RenderTexture renderTexture)
    {
        if (renderTexture == null)
        {
            renderTexture = GenerateDefaultRT();
        }

        if (!renderTexture.IsCreated())
        {
            CreateRT(renderTexture);
        }

        if (renderTexture.IsCreated())
        {
            Utils.ClearRenderTexture(renderTexture);
            _thumbnailCamera.RenderToTexture(renderTexture);
        }
    }

    private RenderTexture GenerateDefaultRT()
    {
        RenderTextureDescriptor desc = new RenderTextureDescriptor(_defaultRTSize, _defaultRTSize, _defaultRTFormat, _defaultRTDepthBufferBits);
        return new RenderTexture(desc);
    }

    private void CreateRT(RenderTexture renderTexture)
    {
        renderTexture.Create();
        renderTexture.filterMode = _textureFilterMode;
    }

    private int GetThumbnailLayer()
    {
        return _thumbnailLayerMask;
    }

    public ThumbnailCamera GetThumbnailCamera()
    {
        return _thumbnailCamera;
    }

    public ThumbnailLight GetThumbnailLight()
    {
        return _thumbnailLight;
    }
}
