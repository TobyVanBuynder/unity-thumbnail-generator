using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using GLTFast;

public class GltfModelLoader : IModelLoader
{
    public async Task<GameObject> Load(string path)
    {
        GameObject go = new GameObject(Path.GetFileName(path));
        go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        
        GltfAsset gltfAsset = go.AddComponent<GltfAsset>();
        gltfAsset.LoadOnStartup = false;
        gltfAsset.InstantiationSettings = new InstantiationSettings{
            SceneObjectCreation = SceneObjectCreation.Never
        };

        await gltfAsset.Load(path);

        return go;
    }
}