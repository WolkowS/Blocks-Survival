using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class Not : MonoBehaviour
    {
        public UnityEvent<bool> m_OnInvoke;

        // =======================================================================
        public void Invoke(bool value)
        {
            m_OnInvoke.Invoke(!value);
        }
    }
}