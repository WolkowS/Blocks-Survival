using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.States
{
    public class GsCheck : MonoBehaviour
    {
        public Gs         _state;
        public UnityEvent _onTrue;
        public UnityEvent _onFalse;
        
        // =======================================================================
        public void Invoke()
        {
            if (_state.IsOpen)
                _onTrue.Invoke();
            else
                _onFalse.Invoke();
        }
    }
}