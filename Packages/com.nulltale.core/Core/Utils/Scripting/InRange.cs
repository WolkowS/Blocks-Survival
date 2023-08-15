using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class InRange : MonoBehaviour
    {
        public Vector2 _range;
        public bool    _invert;
        
        public UnityEvent _onTrue;
        
        // =======================================================================
        public void Invoke(float val)
        {
            var check = val <= _range.y && val >= _range.x;
            
            if (_invert ? !check : check)
                _onTrue.Invoke();
        }
    }
}