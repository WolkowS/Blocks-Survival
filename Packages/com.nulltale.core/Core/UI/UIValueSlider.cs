using System.Globalization;
using CoreLib.Values;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib
{
    public class UIValueSlider : MonoBehaviour
    {
        private Slider m_Slider;
        [SerializeField]
        private Optional<AnimationCurve> m_ValueCurve;
        public PlayerPrefsValue m_PrefsValue;
        public GlobalValue      m_GlobalValue;

        // =======================================================================
        private void Awake()
        {
            m_Slider = GetComponent<Slider>();
            if (m_PrefsValue != null && m_PrefsValue.HasValue)
                m_Slider.value = m_PrefsValue.GetValue<float>();
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
            if (m_PrefsValue != null)
                m_PrefsValue.SetValue(value);

            if (m_ValueCurve)
                value = m_ValueCurve.Value.Evaluate(value);

            if (m_GlobalValue != null)
                switch (m_GlobalValue)
                {
                    case GvFloat f:
                        f.Value = value;
                        break;
                    case GvInt i:
                        i.Value = value.RoundToInt();
                        break;
                    case GvString s:
                        s.Value = value.ToString(CultureInfo.InvariantCulture);
                        break;
                    case GvBool b:
                        b.Value = value >= 0.5f;
                        break;
                }
        }
    }
}