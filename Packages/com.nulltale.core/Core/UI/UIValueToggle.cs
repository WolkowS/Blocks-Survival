using CoreLib.Values;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib
{
    public class UIValueToggle : MonoBehaviour
    {
        private Toggle m_Toggle;

        public PlayerPrefsValue m_PrefsValue;
        public GvBool  m_GlobalValue;

        // =======================================================================
        private void Awake()
        {
            m_Toggle = GetComponent<Toggle>();
            if (m_PrefsValue != null && m_PrefsValue.HasValue)
                m_Toggle.isOn = m_PrefsValue.GetValue<bool>();
        }

        private void OnEnable()
        {
            _update(m_Toggle.isOn);
            m_Toggle.onValueChanged.AddListener(_update);
        }

        private void OnDisable()
        {
            m_Toggle.onValueChanged.RemoveListener(_update);
        }

        private void _update(bool value)
        {
            if (m_PrefsValue != null)
                m_PrefsValue.SetValue(value);

            if (m_GlobalValue != null)
                m_GlobalValue.Value = value;
        }
    }
}