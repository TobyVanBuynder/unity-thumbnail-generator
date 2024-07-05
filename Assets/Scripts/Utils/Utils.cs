using UnityEngine;
using System.Linq;

public static class Utils
{
    public static Bounds GetTotalBounds(Transform parentTransform)
    {
        Bounds bounds = new Bounds(parentTransform.position, Vector3.zero);
        Renderer[] renderers = parentTransform.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }
    public static void FitObjectInOrthographic(Transform objectTransform, Camera camera, ref Vector3[] boundsVertices)
    {
        Bounds totalBounds = GetTotalBounds(objectTransform);
        objectTransform.position = -totalBounds.center;
        if (boundsVertices == null || boundsVertices.Length < 8) boundsVertices = new Vector3[8];
        
        boundsVertices[0].Set(-totalBounds.extents.x, -totalBounds.extents.y, -totalBounds.extents.z);
        boundsVertices[1].Set(-totalBounds.extents.x, -totalBounds.extents.y,  totalBounds.extents.z);
        boundsVertices[2].Set(-totalBounds.extents.x,  totalBounds.extents.y, -totalBounds.extents.z);
        boundsVertices[3].Set(-totalBounds.extents.x,  totalBounds.extents.y,  totalBounds.extents.z);
        boundsVertices[4].Set( totalBounds.extents.x, -totalBounds.extents.y, -totalBounds.extents.z);
        boundsVertices[5].Set( totalBounds.extents.x, -totalBounds.extents.y,  totalBounds.extents.z);
        boundsVertices[6].Set( totalBounds.extents.x,  totalBounds.extents.y, -totalBounds.extents.z);
        boundsVertices[7].Set( totalBounds.extents.x,  totalBounds.extents.y,  totalBounds.extents.z);
        objectTransform.TransformPoints(boundsVertices);
        for (int i = 0; i < boundsVertices.Length; i++)
        {
            boundsVertices[i] = camera.WorldToScreenPoint(boundsVertices[i] + totalBounds.extents);
        }
        float maxSize = Mathf.Max(
            Mathf.Abs(boundsVertices.Max((Vector3 _) => _.x) - boundsVertices.Min((Vector3 _) => _.x)),
            Mathf.Abs(boundsVertices.Max((Vector3 _) => _.y) - boundsVertices.Min((Vector3 _) => _.y))
        );
        camera.orthographicSize = maxSize / Mathf.Min(camera.pixelWidth, camera.pixelHeight);
    }
    public static void ClearRenderTexture(RenderTexture renderTexture)
    {
        RenderTexture prevActive = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = prevActive;
    }

    public static string FixFilePath(string filePath)
    {
        return filePath.Replace("\\", "\\\\");;
    }

    public static string GetExtensionWithoutDot(string filePath)
    {
        return filePath.Substring(filePath.LastIndexOf('.') + 1);
    }
}