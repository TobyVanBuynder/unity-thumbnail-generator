using System.Threading.Tasks;
using UnityEngine;
using GLTFast;

public class GlbModelLoader : IModelLoader
{
    public Task<GameObject> Load(string path)
    {
        return new Task<GameObject>(() =>
        {
            GameObject go = new GameObject();
            go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            GltfAsset gltfAsset = go.AddComponent<GltfAsset>();
            gltfAsset.LoadOnStartup = false;
            gltfAsset.InstantiationSettings = new InstantiationSettings{
                SceneObjectCreation = SceneObjectCreation.Never
            };

            return go;
        });
    }
}