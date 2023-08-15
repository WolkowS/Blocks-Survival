using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreLib
{
    public class InputActionContainer : InputActionBase
    {
        [SerializeField]
        private InputAction m_InputAction;

        // =======================================================================
        private void OnEnable()
        {
            m_InputAction.performed += _performed;
            m_InputAction.started   += _started;
            m_InputAction.canceled  += _cancelled;
            m_InputAction.Enable();
        }

        private void OnDisable()
        {
            m_InputAction.performed -= _performed;
            m_InputAction.started   -= _started;
            m_InputAction.canceled  -= _cancelled;
            m_InputAction.Disable();
        }

        private void _performed(InputAction.CallbackContext context)
        {
            m_Performed?.Invoke(context);
        }

        private void _started(InputAction.CallbackContext context)
        {
            m_Started?.Invoke(context);
        }

        private void _cancelled(InputAction.CallbackContext context)
        {
            m_Cancelled?.Invoke(context);
        }
    }
}