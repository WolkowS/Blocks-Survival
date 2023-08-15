using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.SceneManagement
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder - 1)]
    public class SceneData : MonoBehaviour
    {
        public Dictionary<IdAsset, SceneState> m_States = new Dictionary<IdAsset, SceneState>();

        // =======================================================================
        protected void Awake()
        {
            SceneManager.Instance.m_SceneData.Add(gameObject.scene.handle, this);
        }

        protected void OnDestroy()
        {
            SceneManager.Instance.m_SceneData.Remove(gameObject.scene.handle);
        }
    }
}