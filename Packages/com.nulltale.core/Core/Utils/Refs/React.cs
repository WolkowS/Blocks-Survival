using UnityEngine;

namespace CoreLib
{
    [ExecuteAlways]
    public abstract class React<T> : MonoBehaviour 
    {
        public SignalLink<T> _signalLink;
        public bool          _onEnable;
        
        // =======================================================================
        protected virtual void OnEnable()
        {
            var singnal = _signalLink.Value;
            singnal.React += _react;
            if (_onEnable)
                _react(singnal.Value);
        }

        protected virtual void OnDisable()
        {
            _signalLink.Value.React -= _react;
        }

        protected abstract void _react(T val);
    }
}