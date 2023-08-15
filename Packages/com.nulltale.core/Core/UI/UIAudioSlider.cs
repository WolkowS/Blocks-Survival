using System;
using System.Linq;
using CoreLib.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib
{
    public class UIAudioSlider : MonoBehaviour
    {
        private Slider     m_Slider;
        private MixerValue m_MixerValue;
        [SerializeField]
        private PlayerPrefsValue m_Value;

        // =======================================================================
        private void Awake()
        {
            m_Slider       = GetComponent<Slider>();

            if (m_Value == null)
            {
                Debug.LogWarning($"{gameObject.name} prefs value is null", gameObject);
                return;
            }

            m_MixerValue = SoundManager.Instance.MixerValues.FirstOrDefault(n => n.PrefsValue == m_Value);
            if (m_MixerValue == null)
            {
                Debug.LogWarning($"{gameObject.name} mixer value not found", gameObject);
                return;
            }

            m_Slider.value = m_MixerValue.Value;
        }

        private void OnEnable()
        {
            _update(m_Slider.value);
            m_Slider.onValueChanged.AddListener(_update);
        }

        private void OnDisable()
        {
            m_Slider.onValueChanged.RemoveListener(_update);
        }

        // -----------------------------------------------------------------------
        private void _update(float value)
        {
            if (m_MixerValue != null)
                m_MixerValue.Value = value;
        }
    }
}