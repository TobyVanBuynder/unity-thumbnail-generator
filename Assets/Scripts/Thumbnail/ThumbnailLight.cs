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

    public void RotateY(float angle)
    {
        transform.Rotate(Vector3.up, angle, Space.World);
    }

    public void SetLayerMask(LayerMask layerMask)
    {
        _light.cullingMask = layerMask;
    }
}