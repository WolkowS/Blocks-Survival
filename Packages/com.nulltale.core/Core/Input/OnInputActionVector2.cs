using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    [RequireComponent(typeof(InputActionBase))]
    public class OnInputActionVector2 : MonoBehaviour
    {
        public UnityEvent<Vector2> m_Action;

        // =======================================================================
        private void Awake()
        {
            var action = GetComponent<InputActionBase>();
            action.StartedC   = c => m_Action.Invoke(c.ReadValue<Vector2>());
            action.PerformedC = c => m_Action.Invoke(c.ReadValue<Vector2>());
            action.CancelledC = c => m_Action.Invoke(c.ReadValue<Vector2>());
        }
    }
}