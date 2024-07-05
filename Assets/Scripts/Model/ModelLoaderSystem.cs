using UnityEngine;
using System.IO;
using SFB;

using ModelTypeDictionary = System.Collections.Generic.Dictionary<string, Model.Type>;
using ModelLoaderDictionary = System.Collections.Generic.Dictionary<Model.Type, IModelLoader>;


public class ModelLoaderSystem : MonoBehaviour
{
    ModelTypeDictionary _modelTypeDictionary;
    ModelLoaderDictionary _modelLoaderDictionary;
    ExtensionFilter[] _validFileTypes;

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
        FileSystem.FileTypeBuilder fileTypeBuilder = new FileSystem.FileTypeBuilder();

        AddValidType(Model.Type.GLTF, new GltfModelLoader(), fileTypeBuilder, "GLTF scene", "gltf");
        AddValidType(Model.Type.GLB, new GlbModelLoader(), fileTypeBuilder, "GLTF binary", "glb");

        _validFileTypes = fileTypeBuilder.Build();
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
        GlobalEvents.OnSelectFile += OnSelectFile;
    }

    void OnDisable()
    {
        GlobalEvents.OnSelectFile -= OnSelectFile;
    }

    private void OnSelectFile(string path)
    {
        FileSystem.OpenPanelAsync("Import 3D model", _validFileTypes, path, OnOpenFile);
    }

    private void OnOpenFile(string[] filePaths)
    {
        if (filePaths.Length > 0)
        {
            string filePath = Utils.FixFilePath(filePaths[0]);
            TryLoadModelFromFile(filePath);
        }
    }

    private async void TryLoadModelFromFile(string filePath)
    {
        string extension = Path.GetExtension(filePath);

        if (IsExtensionValid(extension))
        {
            Model.Type modelType = _modelTypeDictionary[extension];
            IModelLoader modelLoader = GetModelLoader(modelType);

            GameObject loadedModelObject = await modelLoader?.Load(filePath);

            GlobalEvents.OnModelLoaded?.Invoke(filePath, loadedModelObject, modelType);
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
