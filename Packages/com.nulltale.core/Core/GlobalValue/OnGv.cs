using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Values
{
    [ExecuteAlways]
    public class OnGv<TType, TGlobalValue> : MonoBehaviour
        where TGlobalValue : GlobalValue, IGlobalValue<TType>
    {
        [SerializeField]
        private TGlobalValue m_GlobalValue;
        
        [SerializeField]
        private  bool m_OnEnable = true;

        [SerializeField]
        private UnityEvent<TType> m_OnValueChanged;

        public TGlobalValue Value => m_GlobalValue;

        // =======================================================================
        private void OnEnable()
        {
            if (m_GlobalValue == null)
                return;

            m_GlobalValue.OnChanged += _onValueChanged;
            
            if (m_OnEnable == false)
                return;
            
            Invoke();
        }

        private void OnDisable()
        {
            if (m_GlobalValue == null)
                return;

            m_GlobalValue.OnChanged -= _onValueChanged;
        }
        
        public void Invoke()
        {
            _onValueChanged(m_GlobalValue.Value);
        }

        private void _onValueChanged(TType value)
        {
            m_OnValueChanged.Invoke(value);
        }
    }
}