using System;
using NaughtyAttributes;
using SoCreator;
using UnityEngine;

namespace CoreLib.Events
{
    [SoCreate(true, priority:-100)]
    public class GeVoid : GlobalEvent
    {
        public event Action OnInvoke;
        private float _time;
        
        // =======================================================================
        [Button]
        public void Invoke()
        {
            if (_time == Time.time)
                return;
            
            _time = Time.time;
            OnInvoke?.Invoke();
        }
    }
}