using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreLib
{
    public abstract class InputActionBase : MonoBehaviour
    {
        protected Action<InputAction.CallbackContext> m_Performed;
        protected Action<InputAction.CallbackContext> m_Started;
        protected Action<InputAction.CallbackContext> m_Cancelled;

        // =======================================================================
        public Action<InputAction.CallbackContext> ActionCallback
        {
            set => m_Performed = value;
        }
        public Action Performed
        {
            set => m_Performed = c => value?.Invoke();
        }
        
        public Action Started
        {
            set => m_Started = c => value?.Invoke();
        }
        
        public Action Cancelled
        {
            set => m_Cancelled = c => value?.Invoke();
        }
        
        public Action<InputAction.CallbackContext> StartedC
        {
            set => m_Started = value.Invoke;
        }

        public Action<InputAction.CallbackContext> PerformedC
        {
            set => m_Performed = value.Invoke;
        }

        public Action<InputAction.CallbackContext> CancelledC
        {
            set => m_Cancelled = value.Invoke;
        }
    }
}