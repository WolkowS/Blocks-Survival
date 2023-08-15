using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace CoreLib
{
    public class InputActionEventSystem : InputActionBase
    {
        [SerializeField]
        private ActionType m_Type;
        public ActionType Type => m_Type;

        // =======================================================================
        [Serializable]
        public enum ActionType
        {
            Cancel,
            Submit,
            Click,
        }

        // =======================================================================
        private void OnEnable()
        {
            var action = getInputAction();
            if (action != null)
            {
                action.performed += _performed;
                action.started   += _started;
                action.canceled  += _cancelled;
            }
        }

        private void OnDisable()
        {
            var action = getInputAction();
            if (action != null)
            {
                action.performed -= _performed;
                action.started   -= _started;
                action.canceled  -= _cancelled;
            }
        }

        private InputAction getInputAction()
        {
            if (UnityEngine.EventSystems.EventSystem.current == null)
                return null;

            var inputModule = UnityEngine.EventSystems.EventSystem.current.GetComponent<InputSystemUIInputModule>();

            if (inputModule == null)
                return null;

            var action = Type switch
            {
                ActionType.Cancel => inputModule.cancel.action,
                ActionType.Submit => inputModule.submit.action,
                ActionType.Click  => inputModule.leftClick.action,
                _                 => throw new ArgumentOutOfRangeException()
            };
            return action;
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