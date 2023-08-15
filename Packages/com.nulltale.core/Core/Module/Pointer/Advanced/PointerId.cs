using System;

namespace CoreLib.Module
{
    public class PointerId : IdAsset
    {
        public int  _priority;
        public int  _state;
        public bool _isActive;
        
        public event Action<bool> OnChanged;
        
        // =======================================================================
        internal void SetActive(bool isActive)
        {
            if (_isActive == isActive)
                return;
            
            _isActive = isActive;
            OnChanged?.Invoke(_isActive);
        }
    }
}