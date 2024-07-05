using UnityEngine;

public class AutoDestroyThumbnailModel : MonoBehaviour
{
    private bool _isThumbnailModel = false;

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
        if (_isThumbnailModel)
        {
            Destroy(modelObj);
            _isThumbnailModel = false;
        }
    }

    private void OnThumbnailLoaded(Texture _)
    {
        _isThumbnailModel = true;
    }
}