using UnityEngine;

namespace CoreLib
{
    [RequireComponent(typeof(SceneLoader))]
    public class SceneReloader : MonoBehaviour
    {
        public void Invoke()
        {
            //CoreLib.SceneManagement.SceneManager.Instance.m_Scenes.firs
            GetComponent<SceneLoader>().Load(gameObject.scene.name);
        }
    }
}