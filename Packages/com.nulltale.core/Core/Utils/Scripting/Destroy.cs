using System;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib.Scripting
{
    [Serializable]
    public class Destroy : MonoBehaviour
    {
        public Optional<Vers<GameObject>> _go;
        [SerializeField]
        private Method m_Method;

        [SerializeField] [ShowIf(nameof(m_Method), Method.Default)]
        private Optional<float> m_Delay;

        // =======================================================================
        [Serializable]
        public enum Method
        {
            Default,
            Immediate
        }

        // =======================================================================
        public void Invoke()
        {
            var go = _go.Enabled ? _go.Value : gameObject;
            _destroy(go);
        }
        
        public void DestroySelf()
        {
            _destroy(gameObject);
        }

        public void DestroyTarget(Object obj)
        {
            _destroy(obj);
        }

        // =======================================================================
        protected void _destroy(Object go)
        {
            // object must exist
            if (go == null)
                return;

            // only in play mode
            if (Application.isPlaying == false)
                return;

            // implement
            switch (m_Method)
            {
                case Method.Default:
                {
                    if (m_Delay.Enabled)
                        Destroy(go, m_Delay.Value);
                    else
                        Destroy(go);
                } break;
                case Method.Immediate:
                {
                    DestroyImmediate(go);
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}