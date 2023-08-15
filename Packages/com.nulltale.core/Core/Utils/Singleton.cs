using UnityEngine;

namespace CoreLib
{
    public class Singleton<TType> : MonoBehaviour, ISingleton
        where TType : Singleton<TType> 
    {
        private static TType s_Instance;
        public static TType Instance
        {
            get
            {
#if UNITY_EDITOR
                if (s_Instance.IsNull())
                    s_Instance = FindObjectOfType<TType>();
#endif
                return s_Instance;
            }
        }

        // =======================================================================
        protected virtual void Awake()
        {
            SetupSingleton();
        }

        public virtual void SetupSingleton()
        {
            s_Instance = (TType)this;
        }
    }

    public interface ISingleton
    {
        void SetupSingleton();
    }
}