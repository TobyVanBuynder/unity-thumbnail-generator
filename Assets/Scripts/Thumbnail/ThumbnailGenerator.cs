using UnityEngine;

public class ThumbnailGenerator : MonoBehaviour
{
    private readonly int _defaultRTSize = 256;
    private readonly RenderTextureFormat _defaultRTFormat = RenderTextureFormat.ARGB32;
    private readonly int _defaultRTDepthBufferBits = 8;
    private readonly Vector3 _thumbnailObjectForward = -Vector3.forward;

    [SerializeField] private Camera _camera;
    [SerializeField] private Light _directionalLight;
    [SerializeField] private Transform _holder;

    private Vector3[] _boundsVertices = null;
    private int _thumbnailLayerMask = -1;

    void Awake()
    {
        _thumbnailLayerMask = LayerMask.NameToLayer("Thumbnail");

        SetLayerMask(GetThumbnailLayer(), transform, true);
        Disable();
    }

    private void Enable()
    {
        _camera.enabled = true;
        _directionalLight.enabled = true;
    }

    private void Disable()
    {
        _camera.enabled = false;
        _directionalLight.enabled = false;
    }

    private int GetThumbnailLayer()
    {
        return _thumbnailLayerMask;
    }

    private void SetLayerMask(LayerMask layerMask, Transform objectTransform, bool includeChildren = false)
    {
        objectTransform.gameObject.layer = layerMask;

        if (includeChildren)
        {
            for (int c = 0; c < objectTransform.childCount; c++)
            {
                SetLayerMask(layerMask, objectTransform.GetChild(c), includeChildren);
            }
        }
    }

    public void GenerateThumbnail(Transform objectTransform, RenderTexture renderTexture = null)
    {
        Transform oldParent = objectTransform.parent;
        Vector3 oldForward = objectTransform.forward;
        LayerMask oldLayerMask = objectTransform.gameObject.layer;

        Enable();

        PrepareObjectForGeneration(objectTransform);
        SetupCamera();
        FitObjectInCamera(objectTransform);
        GenerateTexture(renderTexture);
        RestoreObjectAfterGeneration(objectTransform, oldParent, oldForward, oldLayerMask);

        Disable();
    }

    private void PrepareObjectForGeneration(Transform objectTransform)
    {
        objectTransform.SetParent(_holder, false);
        objectTransform.forward = _thumbnailObjectForward;
        SetLayerMask(GetThumbnailLayer(), objectTransform, true);
    }

    private void RestoreObjectAfterGeneration(Transform objectTransform, Transform oldParent, Vector3 oldForward, LayerMask oldLayerMask)
    {
        objectTransform.forward = oldForward;
        objectTransform.SetParent(oldParent, false);
        SetLayerMask(oldLayerMask, objectTransform, true);
    }

    private void SetupCamera()
    {
        _camera.transform.position = -_camera.transform.forward * 10;
        _camera.aspect = 1;
        _camera.orthographicSize = 1;
    }

    private void FitObjectInCamera(Transform objectTransform)
    {
        Utils.FitObjectInOrthographic(objectTransform, _camera, ref _boundsVertices);
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
            RenderOnCamera(renderTexture);
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
        renderTexture.filterMode = FilterMode.Point;
    }

    private void RenderOnCamera(RenderTexture renderTexture)
    {
        _camera.targetTexture = renderTexture;
        _camera.Render();
        _camera.targetTexture = null;
    }
}
