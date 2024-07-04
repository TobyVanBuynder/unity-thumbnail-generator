using UnityEngine;

public class ThumbnailGeneratorLoader : MonoBehaviour
{
    [SerializeField] private GameObject _thumbnailGeneratorPrefab;

    private ThumbnailGenerator _thumbnailGenerator = null;

    void Awake()
    {
        if (_thumbnailGeneratorPrefab == null)
        {
            enabled = false;
        }
    }

    public ThumbnailGenerator Get()
    {
        if (_thumbnailGenerator == null)
        {
            CreateThumbnailGenerator();
        }

        return _thumbnailGenerator;
    }

    private void CreateThumbnailGenerator()
    {
        _thumbnailGenerator = Instantiate(_thumbnailGeneratorPrefab, transform).GetComponent<ThumbnailGenerator>();
    }
}
