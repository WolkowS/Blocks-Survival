using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    [DefaultExecutionOrder(-1)]
    public class OnStart : MonoBehaviour
    {
        public UnityEvent m_Event;

        // =======================================================================
        private void Start()
        {
            m_Event.Invoke();
        }
    }
}