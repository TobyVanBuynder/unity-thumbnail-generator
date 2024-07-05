using System.Threading.Tasks;
using UnityEngine;

public interface IModelLoader
{
    Task<GameObject> Load(string path);
}