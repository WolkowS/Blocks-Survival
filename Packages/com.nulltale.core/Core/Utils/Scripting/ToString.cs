using System;
using System.Linq;
using CoreLib;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class ToString : MonoBehaviour
    {
        public Optional<string> m_Format = new Optional<string>("{0:N2}", false);

        public UnityEvent<string> m_OnCast;

        // =======================================================================
        public void Invoke(MonoBehaviour iRef)
        {
            switch (iRef)
            {
                case IRefGet<int> refCast:
                    Invoke(refCast.Value);
                    break;
                
                case IRefGet<bool> refCast:
                    Invoke(refCast.Value);
                    break;
                
                case IRefGet<float> refCast:
                    Invoke(refCast.Value);
                    break;
            }
        }
        public void Invoke(float val) => _cast(val);
        public void Invoke(int val) => _cast(val);
        public void Invoke(bool val) => _cast(val);

        private string _cast<T>(T val)
        {
            var result = m_Format.Enabled ? string.Format(m_Format.Value, val) : val.ToString();
            m_OnCast.Invoke(result);
            return result;
        }
    }
}