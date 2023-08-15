using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.SceneManagement
{
    public class ScenePreset :ScriptableObject
    {
        public string  SceneName => m_SceneName;
        [SerializeField] [Scene]
        internal string m_SceneName;
    }
}