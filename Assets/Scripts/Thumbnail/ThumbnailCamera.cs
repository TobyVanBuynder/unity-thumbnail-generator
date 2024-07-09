using UnityEngine;

public class ThumbnailCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private Vector3[] _boundsVertices = null;

    public void Enable()
    {
        _camera.enabled = true;
    }

    public void Disable()
    {
        _camera.enabled = false;
    }

    public void Reset()
    {
        _camera.transform.position = -_camera.transform.forward * 10;
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
}