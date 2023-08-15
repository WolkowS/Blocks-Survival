using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Module
{
    public class OnPointer : MonoBehaviour
    {
        public PointerId  _id;
        public bool       _applyInitial;
        public UnityEvent _onEnable;
        public UnityEvent _onDisable;
        
        // =======================================================================
        private void OnEnable()
        {
            _id.OnChanged += _onChanged;
            
            if (_applyInitial)
                _onChanged(_id._isActive);
        }

        private void OnDisable()
        {
            _id.OnChanged -= _onChanged;
        }

        // =======================================================================
        private void _onChanged(bool isOn)
        {
            (isOn ? _onEnable : _onDisable).Invoke();
        }
    }
}