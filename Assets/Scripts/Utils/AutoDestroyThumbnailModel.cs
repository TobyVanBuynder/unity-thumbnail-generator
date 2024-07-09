using UnityEngine;

public class AutoDestroyThumbnailModel : MonoBehaviour
{
    private GameObject _lastLoadedModel = null;

    void OnEnable()
    {
        GlobalEvents.OnModelLoaded += OnModelLoaded;
    }

    void OnDisable()
    {
        GlobalEvents.OnModelLoaded -= OnModelLoaded;
    }

    private void OnModelLoaded(string _, GameObject modelObj, Model.Type __)
    {
        if (_lastLoadedModel != null)
        {
            Destroy(_lastLoadedModel);
        }

        _lastLoadedModel = modelObj;
    }
}