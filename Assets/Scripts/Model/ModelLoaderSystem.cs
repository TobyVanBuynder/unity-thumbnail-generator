using UnityEngine;
using System.IO;

using ModelTypeDictionary = System.Collections.Generic.Dictionary<string, Model.Type>;
using ModelLoaderDictionary = System.Collections.Generic.Dictionary<Model.Type, IModelLoader>;

public class ModelLoaderSystem : MonoBehaviour
{
    ModelTypeDictionary _modelTypeDictionary;
    ModelLoaderDictionary _modelLoaderDictionary;

    void Awake()
    {
        _modelTypeDictionary = new ModelTypeDictionary();
        _modelLoaderDictionary = new ModelLoaderDictionary();
    }

    void Start()
    {
        RegisterValidTypes();
    }

    private void RegisterValidTypes()
    {
        FileSystem.FileTypeBuilder builder = new FileSystem.FileTypeBuilder();

        AddValidType(Model.Type.GLTF, new GltfModelLoader(), builder, "GLTF scene", "gltf");
    }

    private void AddValidType(Model.Type type, IModelLoader modelLoader, FileSystem.FileTypeBuilder builder, string fileTypeName, params string[] fileTypes)
    {
        foreach (string fileType in fileTypes)
        {
            _modelTypeDictionary.Add(fileType, type);
        }

        _modelLoaderDictionary.Add(type, modelLoader);

        builder.Add(fileTypeName, fileTypes);
    }

    void OnEnable()
    {
        GlobalEvents.OnOpenFile += OnOpenFile;
    }

    void OnDisable()
    {
        GlobalEvents.OnOpenFile -= OnOpenFile;
    }

    private async void OnOpenFile(string path)
    {
        string extension = Path.GetExtension(path);

        if (IsExtensionValid(extension))
        {
            IModelLoader modelLoader = GetModelLoader(_modelTypeDictionary[extension]);
            await modelLoader?.Load(path);
        }
    }

    private bool IsExtensionValid(string extension)
    {
        return _modelTypeDictionary.ContainsKey(extension);
    }

    private IModelLoader GetModelLoader(Model.Type type)
    {
        return IsModelTypeValid(type) ? _modelLoaderDictionary[type] : null;
    }

    private bool IsModelTypeValid(Model.Type type)
    {
        return _modelLoaderDictionary.ContainsKey(type);
    }
}
