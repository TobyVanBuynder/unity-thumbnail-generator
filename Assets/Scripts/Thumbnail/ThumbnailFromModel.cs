using System;
using UnityEngine;

public class ThumbnailFromModel : MonoBehaviour
{
    [SerializeField] ThumbnailGeneratorLoader _generatorLoader;

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

    private void OnModelLoaded(string filePath, GameObject modelObject, Model.Type type)
    {
        if (IsOfTypeGLTF(type))
        {
            FixGltfParentTransform(modelObject);
        }

        if (_thumbnail == null)
        {
            CreateRenderTexture();
        }

        _generatorLoader.Get().GenerateThumbnail(modelObject.transform, _thumbnail);

        GlobalEvents.OnThumbnailLoaded?.Invoke(_thumbnail);
    }

    private bool IsOfTypeGLTF(Model.Type type)
    {
        return type == Model.Type.GLTF || type == Model.Type.GLB;
    }

    private void FixGltfParentTransform(GameObject modelObject)
    {
        Transform parent = modelObject.transform.GetChild(0);
        parent.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        parent.localScale = Vector3.one;
    }

    private void CreateRenderTexture()
    {
        RenderTextureDescriptor desc = new RenderTextureDescriptor(256, 256, RenderTextureFormat.ARGB32, 0);
        _thumbnail = new RenderTexture(desc);
        _thumbnail.Create();
        _thumbnail.filterMode = FilterMode.Point;
    }
}
