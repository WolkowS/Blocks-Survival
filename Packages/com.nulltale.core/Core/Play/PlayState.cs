using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Play
{
    public class PlayState : OnPlayBase
    {
        [MinMaxSlider(0, 1)]
        public Vector2 _onValue;

        private TriBool _isOn;
        private IPlay   _play;
        
        public UnityEvent _onEnter;
        public UnityEvent _onExit;

        // =======================================================================
        protected override void _onPlay(float scale)
        {
            if (scale >= _onValue.x && scale <= _onValue.y)
            {
                if (_isOn == TriBool.True)
                {
                    _onEnter.Invoke();
                    _isOn = TriBool.True;
                }
            }
            else
            {
                if (_isOn == TriBool.False)
                {
                    _onExit.Invoke();
                    _isOn = TriBool.False;
                }
            }
        }
    }
}