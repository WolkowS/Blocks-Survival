using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CoreLib.Scripting
{
    public class ApplyUIControl : MonoBehaviour
    {
        private void OnEnable()
        {
            if (TryGetComponent<Slider>(out var slider))
            {
                slider.onValueChanged.Invoke(slider.value);
            }
            else
            if (TryGetComponent<Toggle>(out var toggle))
            {
                toggle.onValueChanged.Invoke(toggle.isOn);
            }
            else
            if (TryGetComponent<TMP_Dropdown>(out var dropdown))
            {
                dropdown.onValueChanged.Invoke(dropdown.value);
            }
            else
            if (TryGetComponent<Stick>(out var stick))
            {
                stick._onChanged.Invoke(stick.Value);
            }
        }
    }
}