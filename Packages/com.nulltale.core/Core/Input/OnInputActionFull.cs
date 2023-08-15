using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    [RequireComponent(typeof(InputActionBase))]
    public class OnInputActionFull : MonoBehaviour
    {
        public UnityEvent m_Performed;
        public UnityEvent m_Started;
        public UnityEvent m_Cancelled;

        private void Awake()
        {
            var actionBase = GetComponent<InputActionBase>();
            actionBase.Performed = m_Performed.Invoke;
            actionBase.Started   = m_Started.Invoke;
            actionBase.Cancelled = m_Cancelled.Invoke;
        }
    }
}