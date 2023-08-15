using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class IntMul : MonoBehaviour
    {
        public Vers<int> m_First;
        public Vers<int> m_Second;

        public UnityEvent<int> m_OnInvoke;

        // =======================================================================
        public void Invoke()
        {
            m_OnInvoke.Invoke(m_First.Value * m_Second.Value);
        }
    }
}