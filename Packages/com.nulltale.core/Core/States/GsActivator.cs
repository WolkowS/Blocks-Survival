using UnityEngine;
using UnityEngine.Scripting;

namespace CoreLib.States
{
    [Preserve]
    public class GsActivator : MonoBehaviour
    {
        [SerializeField]
        private GlobalStateBase m_State;
        private StateHandle     m_Handle;

        public GlobalStateBase State
        {
            get => m_Handle.Group;
            set
            {
                m_State = value;
                m_Handle.Group = value;
                m_Handle.IsOpen = isActiveAndEnabled;
            }
        }

        // =======================================================================
        private void Awake()
        {
            m_Handle = new StateHandle(m_State);
        }

        protected void OnEnable()
        {
            m_Handle.Open();
        }

        private void OnDisable()
        {
            m_Handle.Close();
        }
    }
}