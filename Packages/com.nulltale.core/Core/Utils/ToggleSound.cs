using CoreLib.Sound;
using UnityEngine;

namespace CoreLib
{
    public class ToggleSound : MonoBehaviour, IToggle
    {
        public  Optional<SoundAsset> _on;
        public  Optional<SoundAsset> _off;
        private bool       _isOn;

        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (_isOn == value)
                    return;
                
                _isOn = value;
                var sound = _isOn ? _on : _off;
                if (sound.Enabled == false)
                    return;
                
                sound.Value.Play();
            }
        }

        public void On() => IsOn = true;
        public void Off() => IsOn = false;
    }

}