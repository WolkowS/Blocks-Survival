using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class ReactAdsr : React<float>
    {
        public  Vers<Adsr> _adsr;
        public  float      _scale = 1;
        private bool       _pressed;
        private float      _vel;
        private float      _time;

        public UnityEvent<float> _onAdsr;
        
        // =======================================================================
        protected override void OnDisable()
        {
            _pressed = false;
            _vel     = 0f;
            
            base.OnDisable();
        }

        protected override void _react(float val)
        {
            _pressed = val > 0f;
            if (_pressed)
                _vel = val;
        }

        public void Update()
        {
            if (_adsr.Value.IsComplete && _pressed == false)
                return;
                    
            var adsr = _adsr.Value.Update(_pressed, ref _time, Time.deltaTime) * _scale * _vel;
            _onAdsr.Invoke(adsr);
        }

    }
}