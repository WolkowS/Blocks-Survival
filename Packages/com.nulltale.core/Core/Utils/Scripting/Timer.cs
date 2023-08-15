using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class Timer : MonoBehaviour
    {
        public Vers<float> _duration;
        public Vers<float> _current;
        public bool        _realTime;
        
        public UnityEvent<float> _scalar;
        public UnityEvent        _onExpired;
        private bool             _expired;
        
        // =======================================================================
        private void OnEnable()
        {
            Restore();
        }
        
        public void Invoke()
        {
            Restore();
        }
        
        public void Invoke(float sec)
        {
            _duration.Value = sec;
            Restore();
        }

        public void Restore()
        {
            _expired = false;
            _current.Value = 0f;
        }
        
        private void Update()
        {
            if (_expired)
                return;
            
            _scalar.Invoke((_current.Value / _duration.Value).Clamp01());
            if (_current.Value >= _duration.Value)
            {
                _expired = true;
                _onExpired.Invoke();
            }
            
            var delta = _realTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _current.Value = (_current.Value + delta).Clamp(0, _duration.Value);
        }
    }
}