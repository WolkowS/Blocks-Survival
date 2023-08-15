using UnityEngine;

namespace CoreLib.Midi
{
    public class ReactNote : MonoBehaviour
    {
        public Note              _note;
        public SignalLink<float> _signal;
        
        // =======================================================================
        protected virtual void OnEnable()
        {
            _signal.Value.React += _react;
        }

        protected virtual void OnDisable()
        {
            _signal.Value.React -= _react;
        }

        protected void _react(float val)
        {
            _note.OnNote(val);
        }
    }
}