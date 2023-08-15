using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class RangeState : MonoBehaviour
    {
        public bool             _isOn;
        [RangeVec2]
        public Vector2          _range;
        public bool             _applyInitial;
        public bool             _invert;
        public UnityEvent<bool> _onChanged;
        
        // =======================================================================
        private void Awake()
        {
            if (_applyInitial)
                _onChanged.Invoke(_invert ? !_isOn : _isOn);
        }

        public void Invoke(float val)
        {
            var check = val >= _range.x && val <= _range.y;
                
            if (check == _isOn)
                return;
            
            _isOn = check;
            _onChanged.Invoke(_invert ? !_isOn : _isOn);
        }
    }
}