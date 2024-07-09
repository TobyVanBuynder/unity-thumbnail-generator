using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using GLTFast;

public class GltfModelLoader : IModelLoader
{
    InstantiationSettings _instantiationSettings;
    ImportSettings _importSettings;
    bool _streamLoadedAssets;

    public GltfModelLoader()
    {
        _instantiationSettings = new InstantiationSettings{
            SceneObjectCreation = SceneObjectCreation.Never
        };
        _importSettings = new ImportSettings{
            AnimationMethod = AnimationMethod.None
        };
        _streamLoadedAssets = false;
    }

    public GltfModelLoader(InstantiationSettings instantiationSettings, ImportSettings importSettings, bool streamLoadedAssets = false)
    {
        _instantiationSettings = instantiationSettings;
        _importSettings = importSettings;
        _streamLoadedAssets = streamLoadedAssets;
    }

    public async Task<GameObject> Load(string path)
    {
        GameObject go = new GameObject(Path.GetFileName(path));
        go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        
        GltfAsset gltfAsset = go.AddComponent<GltfAsset>();
        gltfAsset.InstantiationSettings = _instantiationSettings;
        gltfAsset.ImportSettings = _importSettings;
        gltfAsset.StreamingAsset = _streamLoadedAssets;

        await gltfAsset.Load(path);

        return go;
    }
}