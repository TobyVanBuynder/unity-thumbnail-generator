using System.Threading.Tasks;
using UnityEngine;

public class GlbModelLoader : IModelLoader
{
    public Task<GameObject> Load(string path)
    {
        return new Task<GameObject>(() => {
            var gameObject = new GameObject();
            return gameObject;
        });
    }
}