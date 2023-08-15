using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    [RequireComponent(typeof(InputActionBase))]
    public class OnInputActionVector2Full : MonoBehaviour
    {
        public UnityEvent<Vector2> m_Started;
        public UnityEvent<Vector2> m_Performed;
        public UnityEvent<Vector2> m_Cancelled;

        private void Awake()
        {
            var action = GetComponent<InputActionBase>();
            action.StartedC   = c => m_Started.Invoke(c.ReadValue<Vector2>());
            action.PerformedC = c => m_Performed.Invoke(c.ReadValue<Vector2>());
            action.CancelledC = c => m_Cancelled.Invoke(c.ReadValue<Vector2>());
        }
    }
}