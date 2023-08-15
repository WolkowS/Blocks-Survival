using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CoreLib.SceneManagement
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder)]
    public class SceneInitializer : MonoBehaviour
    {
        public  static Dictionary<Scene, SceneInitializer> s_Initializers = new Dictionary<Scene, SceneInitializer>();
        
        public  UnityEvent<SceneArgs>       m_OnInit;
        private SceneInitializerPass[]      m_AwakePasses;
        private SceneInitializerPass[]      m_StartPasses;
        private SceneInitializerPass[]      m_LatePasses;
        private SceneArgs                   m_Args;
        private List<(int, IInitializable)> m_Init = new List<(int, IInitializable)>(32);
        private bool                        m_Initialized;
        private bool                        m_LateUpdateRequested;

        // =======================================================================
        private void Awake()
        {
            if (m_Args != null)
                m_Args.transform.SetParent(transform);
            
            s_Initializers.Add(gameObject.scene, this);
            
            var sceneName = gameObject.scene.name;
            var scenePreset = SceneManager.Instance.m_Scenes.Values.FirstOrDefault(n => n.SceneName == sceneName);

            var lookup = transform.GetChildren<SceneInitializerPass>().ToLookup(n => n.m_Stage);

            // if key is not presented in lookup, empty sequence was return
            m_AwakePasses = lookup[SceneInitializerPass.Stage.Awake].OrderBy(n => n.transform.GetSiblingIndex()).ToArray();
            m_StartPasses = lookup[SceneInitializerPass.Stage.Start].OrderBy(n => n.transform.GetSiblingIndex()).ToArray();
            m_LatePasses  = lookup[SceneInitializerPass.Stage.Late].OrderBy(n => n.transform.GetSiblingIndex()).ToArray();

            m_Args = SceneArgs.Take(scenePreset);

            m_OnInit.Invoke(m_Args);

            foreach (var pass in m_AwakePasses)
                pass.Invoke(m_Args);
        }

        private IEnumerator Start()
        {
            foreach (var pass in m_StartPasses)
                pass.Invoke(m_Args);

            yield return null;

            foreach (var pass in m_LatePasses)
                pass.Invoke(m_Args);
            
            var initList = m_Init.OrderBy(n => n.Item1).Select(n => n.Item2).ToArray();
            m_Init.Clear();
            foreach (var init in initList)
                init.Init();

            m_Initialized = true;
        }

        private void OnDestroy()
        {
            s_Initializers.Remove(gameObject.scene);
        }
        
        private void _requestLateUpdate()
        {
            if (m_LateUpdateRequested)
                return;
            
            m_LateUpdateRequested = true;
                
            StartCoroutine(_lateUpdate());
            
            // -----------------------------------------------------------------------
            IEnumerator _lateUpdate()
            {
                yield return new WaitForEndOfFrame();
                
                m_LateUpdateRequested = false;
                
                var initList = m_Init.OrderBy(n => n.Item1).Select(n => n.Item2).ToArray();
                m_Init.Clear();
                foreach (var init in initList)
                    init.Init();
            }
        }

        public static void AddInitializer(IInitializable init, int priority = 0)
        {
            var comp = init as Component;
            if (comp == null)
                return;
            
            if (s_Initializers.TryGetValue(comp.gameObject.scene, out var sceneInit) == false)
                return;
            
            if (sceneInit.m_Initialized == false)
            {
                sceneInit.m_Init.Add((priority, init));
            }
            else
            {
                sceneInit._requestLateUpdate();
                sceneInit.m_Init.Add((priority, init));
            }
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void _clear() => s_Initializers?.Clear();
#endif
    }
}