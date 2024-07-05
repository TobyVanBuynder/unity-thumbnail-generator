using UnityEngine;

public class AutoDestroyThumbnailModel : MonoBehaviour
{
    GameObject _someModelObject = null;

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
        _someModelObject = modelObj;
    }

    private void OnThumbnailLoaded(Texture _)
    {
        if (_someModelObject != null)
        {
            Destroy(_someModelObject);
        }
    }
}