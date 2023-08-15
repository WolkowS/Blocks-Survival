using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class BoolCheck : MonoBehaviour
    {
        public UnityEvent m_OnTrue;
        public UnityEvent m_OnFalse;

        // =======================================================================
        public void Invoke(bool value)
        {
            if (value)
                m_OnTrue.Invoke();
            else 
                m_OnFalse.Invoke();
        }
    }
}