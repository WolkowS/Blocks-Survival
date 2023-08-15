using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.SceneManagement
{
    public class SceneArgs : MonoBehaviour
    {
        private ScenePreset  m_SceneID;

        // =======================================================================
        public class AutoRelease : MonoBehaviour
        {
            private float   m_TimeLeft;

            // =======================================================================
            public void Awake()
            {
                m_TimeLeft = SceneManager.Instance.m_SceneArgsLifetime.Value;
            }

            private void Update()
            {
                if (m_TimeLeft <= 0)
                {
                    var args = GetComponent<SceneArgs>();
                    
                    if (SceneManager.Instance.m_SceneArgs.TryGetValue(args.m_SceneID, out var list))
                        list.Remove(args);

                    Destroy(args.gameObject);
                    return;
                }
                
                m_TimeLeft -= Time.unscaledDeltaTime;
            }
        }

        // =======================================================================
        public void Rise(ScenePreset scene)
        {
            var copy = Instantiate(gameObject);

            // move to the buffer
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(copy, Core.Instance.m_BufferScene);

            // add auto release components
            if (SceneManager.Instance.m_SceneArgsLifetime)
                copy.AddComponent<AutoRelease>();

            var args = copy.GetComponent<SceneArgs>();
            args.m_SceneID = scene;

            Push(scene, args);
        }
        
        // =======================================================================
        public static SceneArgs Take(ScenePreset scene)
        {
            // pop up arguments from dictionary
            if (scene != null && SceneManager.Instance.m_SceneArgs.TryGetValue(scene, out var list) && list.IsEmpty() == false)
            {
                var result = list[0];
                list.RemoveAt(0);

                if (SceneManager.Instance.m_SceneArgsLifetime)
                    Destroy(result.GetComponent<AutoRelease>());

                return result;
            }

            return default;
        }

        private static void Push(ScenePreset scene, SceneArgs args)
        {
            if (args == null)
                return;

            if (SceneManager.Instance.m_SceneArgs.TryGetValue(scene, out var list) == false)
            {
                list = new List<SceneArgs>();
                SceneManager.Instance.m_SceneArgs.Add(scene, list);
            }

            list.Add(args);
        }
    }
}