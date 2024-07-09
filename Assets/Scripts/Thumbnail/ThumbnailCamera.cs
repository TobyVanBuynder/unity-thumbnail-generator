using UnityEngine;

public class ThumbnailCamera : MonoBehaviour
{
    private readonly float _initialDistance = 10;

    private Camera _camera = null;
    private Vector3[] _boundsVertices = null;

    void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    public void Enable()
    {
        _camera.enabled = true;
    }

    public void Disable()
    {
        _camera.enabled = false;
    }

    public void ResetCamera(Transform objectTransform)
    {
        transform.position = (-transform.forward * _initialDistance) + objectTransform.position;
        _camera.aspect = 1;
        _camera.orthographicSize = 1;
    }

    public void FitObjectInCamera(Transform objectTransform)
    {
        Utils.FitObjectInOrthographic(objectTransform, _camera, ref _boundsVertices);
    }

    public void RenderToTexture(RenderTexture renderTexture)
    {
        _camera.targetTexture = renderTexture;
        _camera.Render();
        _camera.targetTexture = null;
    }

    public void RotateY(float angle)
    {
        transform.Rotate(Vector3.up, angle, Space.World);
    }

    public void SetLayerMask(LayerMask layerMask)
    {
        _camera.cullingMask = layerMask;
    }
}