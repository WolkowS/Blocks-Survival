using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreLib
{
    public class InputActionLink : InputActionBase
    {
        [SerializeField]
        private InputActionReference m_ActionReference;

        // =======================================================================
        private void OnEnable()
        {
            m_ActionReference.action.performed += _performed;
            m_ActionReference.action.started   += _started;
            m_ActionReference.action.canceled  += _cancelled;
            m_ActionReference.action.Enable();
        }

        private void OnDisable()
        {
            m_ActionReference.action.performed -= _performed;
            m_ActionReference.action.started   -= _started;
            m_ActionReference.action.canceled  -= _cancelled;
            m_ActionReference.action.Disable();
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