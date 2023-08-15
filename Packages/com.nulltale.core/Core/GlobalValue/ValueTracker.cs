using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Values
{
    public class ValueTracker<TValue> : MonoBehaviour
    {
        public Vers<TValue>       _value;
        public UnityEvent<TValue> _onUpdate;
        
        // =======================================================================
        private void Update()
        {
            _onUpdate.Invoke(_value.Value);
        }
    }
}