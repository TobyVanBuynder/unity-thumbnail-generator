using UnityEngine;

public class ThumbnailLight : MonoBehaviour
{
    Light _light;

    void Awake()
    {
        _light = GetComponent<Light>();
    }

    public void Enable()
    {
        _light.enabled = true;
    }

    public void Disable()
    {
        _light.enabled = false;
    }
}