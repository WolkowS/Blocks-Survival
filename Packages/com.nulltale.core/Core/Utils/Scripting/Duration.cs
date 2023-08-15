using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class Duration : MonoBehaviour
    {
        public Ref<float>    _duration;
        public Signal<float> _time;
        public Signal<float> _scale;

        public UnityEvent _onComplete;
        
        // =======================================================================
        private void Update()
        {
            if (_time.Value == _duration.Value)
                _onComplete.Invoke();
            
            _time.Value  = (_time.Value + Time.deltaTime).ClampUp(_duration.Value);
            _scale.Value = (_time.Value / _duration.Value).Clamp01();
        }
    }
}