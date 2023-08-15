using UnityEngine.Events;

namespace CoreLib
{
    public class ReactBool : React<bool>
    {
        public UnityEvent<bool> _onInvoke;
        
        // =======================================================================
        protected override void _react(bool val)
        {
            _onInvoke.Invoke(val);
        }
    }
}