using UnityEngine;

public class AutoDestroyThumbnailModel : MonoBehaviour
{
    private GameObject _thumbnailModel = null;

    void OnEnable()
    {
        GlobalEvents.OnModelLoaded += OnModelLoaded;
        GlobalEvents.OnThumbnailLoaded += OnThumbnailLoaded;
    }

    void OnDisable()
    {
        GlobalEvents.OnModelLoaded -= OnModelLoaded;
        GlobalEvents.OnThumbnailLoaded -= OnThumbnailLoaded;
    }

    private void OnModelLoaded(string _, GameObject modelObj, Model.Type __)
    {
        _thumbnailModel = modelObj;
    }

    private void OnThumbnailLoaded(Texture _)
    {
        if (_thumbnailModel != null)
        {
            Destroy(_thumbnailModel);
            _thumbnailModel = null;
        }
    }
}