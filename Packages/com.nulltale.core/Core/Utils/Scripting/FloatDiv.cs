using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class FloatDiv : MonoBehaviour
    {
        public Vers<float>     m_Value;
        public Optional<float> m_ValueMul = new Optional<float>(1f);

        public UnityEvent<float> m_OnInvoke;

        // =======================================================================
        public void Invoke(float val)
        {
            m_OnInvoke.Invoke(val / (m_Value.Value * m_ValueMul.Value));
        }
    }
}