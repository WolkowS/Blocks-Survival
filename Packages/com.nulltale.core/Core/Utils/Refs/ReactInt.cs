using UnityEngine.Events;

namespace CoreLib
{
    public class ReactInt : React<int>
    {
        public UnityEvent<int> _onInvoke;
        
        // =======================================================================
        protected override void _react(int val)
        {
            _onInvoke.Invoke(val);
        }
    }
}