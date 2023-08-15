using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class Counter : MonoBehaviour, IRef<int>
    {
        public Vers<int> _value;
        
        public int Value
        {
            get => _value.Value;
            set
            {
                if (_value.Value == value)
                    return;
                
                _value.Value = value;
                _onChange.Invoke(_value.Value);
            }
        }

        public UnityEvent<int> _onChange;
        
        // =======================================================================
        public void Invoke()
        {
            _onChange.Invoke(Value);
        }
        
        public void Increment()
        {
            Value ++;
        }
        
        public void Decrement()
        {
            Value --;
        }
    }
}