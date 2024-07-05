using UnityEngine;

public class Debugger : MonoBehaviour
{
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

    private void OnModelLoaded(string filePath, GameObject @object, Model.Type type)
    {
        Debug.Log($"Loaded model from: {filePath}");
        Debug.Log($"With Type: {type.ToString()}");
    }

    private void OnThumbnailLoaded(Texture texture)
    {
        Debug.Log($"Loaded thumbnail for model");
    }
}
