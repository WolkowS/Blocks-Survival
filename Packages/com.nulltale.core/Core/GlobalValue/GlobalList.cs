using System;
using System.Collections.Generic;
using NaughtyAttributes;
using SoCreator;
using UnityEngine;

namespace CoreLib.Values
{
    [SoCreate]
    public abstract class GlobalList<T> : GlobalValueReadonly<List<T>>
    {
        [ResizableTextArea]
        public string  _note;
        
        public event Action<T> OnAdd;
        public event Action<T> OnRemove;
        
        // =======================================================================
        public override void Init()
        {
            Value.Clear();
        }

        internal override string Serialize()
        {
            return string.Empty;
        }

        internal override void Deserialize(string data)
        {
            return;
        }

        public void Add(T item)
        {
            Value.Add(item);
            OnAdd?.Invoke(item);
        }
        
        public void Remove(T item)
        {
            Value.Remove(item);
            OnRemove?.Invoke(item);
        }
    }
}