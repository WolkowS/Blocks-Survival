using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace CoreLib.SceneManagement
{
    public interface ISceneLoader
    {
        AsyncOperationHandle<SceneInstance> Load();
        void Unload();
    }
}