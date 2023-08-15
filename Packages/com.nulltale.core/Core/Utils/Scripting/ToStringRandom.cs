using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class ToStringRandom : MonoBehaviour
    {
        public string[] m_Format;

        public UnityEvent<string> m_OnCast;
        
        private string _last;

        // =======================================================================
        public void Invoke(float val) => _cast(val);
        public void Invoke(int val) => _cast(val);
        public void Invoke(bool val) => _cast(val);

        private string _cast<T>(T val)
        {
            if (m_Format.Length > 1)
                _last = m_Format.Except(_last).Random();

            var result = string.Format(_last, val);
            m_OnCast.Invoke(result);
            return result;
        }
    }
}