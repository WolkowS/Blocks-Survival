using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CoreLib
{
    public class ProgressBar : MonoBehaviour
    {
        [Range(0, 1)]
        public float _value;
        
        public Image             _fill;
        public UnityEvent<float> _onChanged;
        
        public float Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                
                _value = value;
                _fill.fillAmount = value;
                _onChanged.Invoke(value);
            }
        }

        private void OnValidate()
        {
            _fill.fillAmount = _value;
        }
    }
}