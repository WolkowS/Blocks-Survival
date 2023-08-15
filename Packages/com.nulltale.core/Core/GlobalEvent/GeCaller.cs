using System;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Events
{
    public abstract class GeCaller : MonoBehaviour
    {
        [Serializable]
        public enum Mode
        {
            GlobalValue,
            RefLink,
            Native,
        }
    }
    
    public class GeCaller<T> : GeCaller
    {
        public Mode _mode;
        [ShowIf(nameof(_mode), Mode.GlobalValue)]
        public GlobalValue<T> _global;
        [ShowIf(nameof(_mode), Mode.RefLink)]
        public RefLinkId<T> _ref;
        [ShowIf(nameof(_mode), Mode.Native)]
        public T _value;
        
        // =======================================================================
        public void Invoke(GlobalEvent<T> globalEvent)
        {
            var value = _mode switch
            {
                Mode.GlobalValue => _global.Value,
                Mode.RefLink     => _ref.Value,
                Mode.Native      => _value,
                _                => throw new ArgumentOutOfRangeException()
            };
            
            globalEvent.Invoke(_value);
        }
    }
}