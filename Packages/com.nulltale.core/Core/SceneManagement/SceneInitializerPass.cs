using System;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.SceneManagement
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder - 1)]
    public class SceneInitializerPass : MonoBehaviour
    {
        public Stage                 m_Stage;
        public UnityEvent<SceneArgs> m_OnInit;

        // =======================================================================
        [Serializable]
        public enum Stage
        {
            Awake,
            Start,
            Late
        }

        // =======================================================================
        public virtual void Invoke(SceneArgs args)
        {
            m_OnInit.Invoke(args);
        }
    }
    
    public abstract class SceneInitializerPass<TArgs> : SceneInitializerPass
        where TArgs : SceneArgs
    {
        public override void Invoke(SceneArgs args)
        {
            base.Invoke(args);

            if (args is TArgs argsCast)
                Invoke(argsCast);
        }

        protected abstract void Invoke(TArgs args);
    }
}