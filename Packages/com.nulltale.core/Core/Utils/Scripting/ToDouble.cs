using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class ToDouble : MonoBehaviour
    {
        public UnityEvent<double> m_OnInvoke;

        // =======================================================================
        public void Invoke(float val)
        {
            m_OnInvoke.Invoke(val);
        }
    }
}