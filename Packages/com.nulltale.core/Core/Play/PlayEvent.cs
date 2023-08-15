using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Play
{
    public class PlayEvent : OnPlayBase
    {
        [MinMaxSlider(0, 1)]
        public Vector2 _onValue;

        private bool  _isOn;
        private IPlay _play;
        
        public UnityEvent _onInvoke;

        // =======================================================================
        protected override void _onPlay(float scale)
        {
            if (scale >= _onValue.x && scale <= _onValue.y)
            {
                if (_isOn == false)
                {
                    _onInvoke.Invoke();
                    _isOn = true;
                }
            }
            else
            {
                _isOn = false;
            }
        }
    }
}