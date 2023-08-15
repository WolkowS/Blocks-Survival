using CoreLib.Values;
using UnityEngine;

namespace CoreLib.States
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder + 1)]
    public class GsSlave : MonoBehaviour
    {
        [SerializeField]
        private GlobalStateBase           m_State;
        public Optional<Vers<GameObject>> m_Slave;

        public bool _invert;

        // =======================================================================
        private void Awake()
        {
            m_State.OnOpen  += _onOpen;
            m_State.OnClose += _onClose;

            if (m_State.IsOpen)
                _onOpen();
            else
                _onClose();
        }

        private void OnDestroy()
        {
            m_State.OnOpen  -= _onOpen;
            m_State.OnClose -= _onClose;
        }

        // =======================================================================
        private void _onOpen()
        {
            var isOpen = !_invert;
            (m_Slave.Enabled ? m_Slave.Value.Value : gameObject).SetActive(isOpen);
        }
        
        private void _onClose()
        {
            var isOpen = _invert;
            (m_Slave.Enabled ? m_Slave.Value.Value : gameObject).SetActive(isOpen);
        }
    }
}