using UnityEngine;
using GLTFast;
using System;

public class ThumbnailGenerator : MonoBehaviour
{
    static int _DefaultRTSize = 256;

    [SerializeField] Camera _camera;
    [SerializeField] Light _directionalLight;
    [SerializeField] Transform _holder;

    Vector3 _thumbnailObjectForward = -Vector3.forward;
    Vector3[] _boundsVertices = null;
    int _thumbnailLayerMask;

    void Awake()
    {
        _thumbnailLayerMask = LayerMask.NameToLayer("Thumbnail");

        SetLayerMask();
        Disable();
    }

    private void SetLayerMask()
    {
        gameObject.layer = GetThumbnailLayer();
        _camera.gameObject.layer = GetThumbnailLayer();
        _directionalLight.gameObject.layer = GetThumbnailLayer();
        _holder.gameObject.layer = GetThumbnailLayer();
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

    public void GenerateThumbnail(Transform objectTransform, RenderTexture renderTexture = null)
    {
        Enable();

        Transform oldParent = objectTransform.parent;
        Vector3 oldForward = objectTransform.forward;

        objectTransform.SetParent(_holder, false);
        objectTransform.forward = _thumbnailObjectForward;

        SetupCamera();
        Utils.FitObjectInOrthographic(objectTransform, _camera, ref _boundsVertices);

        Generate(renderTexture);

        objectTransform.forward = oldForward;
        objectTransform.SetParent(oldParent, false);

        Disable();
    }

    private void SetupCamera()
    {
        _camera.transform.position = -_camera.transform.forward * 10;
        _camera.aspect = 1;
        _camera.orthographicSize = 1;
    }

    private void Generate(RenderTexture renderTexture)
    {
        if (renderTexture == null)
        {
            renderTexture = GenerateDefaultRT();
        }
        else if (!renderTexture.IsCreated())
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
        RenderTextureDescriptor desc = new RenderTextureDescriptor(_DefaultRTSize, _DefaultRTSize, RenderTextureFormat.ARGB32, 0);
        RenderTexture renderTexture = new RenderTexture(desc);
        CreateRT(renderTexture);
        return renderTexture;
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
