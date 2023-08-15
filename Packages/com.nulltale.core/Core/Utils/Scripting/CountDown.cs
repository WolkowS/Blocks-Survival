using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class CountDown : MonoBehaviour
    {
        public Ref<int> _count;

        public UnityEvent<int> _onTick;
        public UnityEvent      _onExpired;

        // =======================================================================
        public void Invoke()
        {
            if (_count.Value <= 0)
                return;
            
            _count.Value --;
            _onTick.Invoke(_count.Value);
            if (_count.Value <= 0)
                _onExpired.Invoke();
        }
    }
}