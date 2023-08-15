using UnityEngine;

namespace CoreLib.Sound
{
    public class MixerValue : MonoBehaviour
    {
        [SerializeField] [AudioMixerParameter]
        private string _exposedValue;

        [SerializeField]
        private Optional<PlayerPrefsValue> _playerPrefs;
        private float   _value;
        
        [SerializeField]
        private AnimationCurve _curveValue = new AnimationCurve(new Keyframe(0, 0, 5.64f, 5.64f, 0, 0.1279f), new Keyframe(1, 1, 0.008f, 0.008f, 0.8f, 0));
        [SerializeField]
        private Vector2 _curveRange = new Vector2(-80.0f, 0.0f);

        public float Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                
                _value = value;
                
                if (_playerPrefs.Enabled)
                    _playerPrefs.Value.SetValue(_value);

                SoundManager.Instance.Mixer.SetFloat(_exposedValue, Mathf.Lerp(_curveRange.x, _curveRange.y, value));
            }
        }

        public PlayerPrefsValue PrefsValue => _playerPrefs;

        // =======================================================================
        public void Init()
        {
            if (_playerPrefs.Enabled && _playerPrefs.Value.HasValue)
                Value = _playerPrefs.Value.GetValue<float>();
        }
    }
}