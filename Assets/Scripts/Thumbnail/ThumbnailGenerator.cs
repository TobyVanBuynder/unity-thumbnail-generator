using UnityEngine;

public class ThumbnailGenerator : MonoBehaviour
{
    [Header("Adjustable")]
    [SerializeField] private string _thumbnailLayerName = "Thumbnail";

    [Header("Linked (DO NOT TOUCH)")]
    [SerializeField] private ThumbnailCamera _thumbnailCamera;
    [SerializeField] private ThumbnailLight _thumbnailLight;

    private readonly int _defaultRTSize = 256;
    private readonly RenderTextureFormat _defaultRTFormat = RenderTextureFormat.ARGB32;
    private readonly int _defaultRTDepthBufferBits = 8;
    private FilterMode _defaultRTFilterMode = FilterMode.Point;
    private readonly Vector3 _thumbnailObjectForward = -Vector3.forward;

    private int _thumbnailLayerMask;

    void Awake()
    {
        if (_thumbnailCamera == null || _thumbnailLight == null)
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
            Vector3 oldForward = objectTransform.forward;
            LayerMask oldLayerMask = objectTransform.gameObject.layer;
            bool oldIsActive = objectTransform.gameObject.activeSelf;

            Enable();

            SetObjectProperties(objectTransform, _thumbnailObjectForward, GetThumbnailLayer(), true);

            SetupCamera(objectTransform);
            GenerateTexture(renderTexture);

            SetObjectProperties(objectTransform, oldForward, oldLayerMask, oldIsActive);

            Disable();
        }
    }

    private void SetObjectProperties(Transform objectTransform, Vector3 forward, LayerMask layerMask, bool isActive)
    {
        objectTransform.gameObject.SetActive(isActive);
        objectTransform.forward = forward;
        Utils.SetLayerMask(layerMask, objectTransform, true);
    }

    private void SetupCamera(Transform objectTransform)
    {
        _thumbnailCamera.ResetCamera(objectTransform);
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
        renderTexture.filterMode = _defaultRTFilterMode;
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
