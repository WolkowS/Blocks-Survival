using System.Collections;
using CoreLib;
using CoreLib.SceneManagement;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace CoreLib
{
    public class SceneLoader : MonoBehaviour
    {
        public ScenePreset                m_Scene;
        public bool                       m_Leave = true;
        public Optional<TransitionPreset> m_Transition;
        public Optional<SceneArgs>        m_Args;
        public bool                       m_Once;
        private bool                      m_Invoked;

        // =======================================================================
        [Button("Load", EButtonEnableMode.Playmode)]
        public void Invoke()
        {
            if (m_Once && m_Invoked)
                return;
            
            m_Invoked = true;
            
            if (m_Args.Enabled)
                m_Args.Value.Rise(m_Scene);

            Load(m_Scene.SceneName);
        }

        public void Load(string sceneName)
        {
            if (m_Transition)
            {
                var transition = Instantiate(m_Transition.Value.m_Prefab);
                transition.Play(_load, _unload, null);
            }
            else
            {
                Core.Instance.StartCoroutine(_immediate());
            }

            // -----------------------------------------------------------------------
            IEnumerator _load()
            {
                yield return null;
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }

            IEnumerator _unload()
            {
                yield return null;

                if (m_Leave == false)
                    yield break;

                yield return SceneManager.UnloadSceneAsync(gameObject.scene);
            }

            IEnumerator _immediate()
            {
                yield return _unload();
                yield return _load();
            }
        }
    }
}