using System;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class Repeater : MonoBehaviour
    {
        [Label("Time")]
        public Vers<float> _perSec;
        public Mode        _mode = Mode.Interval;

        public  bool       _realTime;
        public  UnityEvent _onInvoke;
        [HideInInspector]
        public Ref<float>  _scale;
        private float      _timer;

        // =======================================================================
        public enum Mode
        {
            PerSec,
            Interval
        }

        // =======================================================================
        private void Update()
        {
            if (_perSec.Value <= 0)
                return;
            
            var interval = _mode switch
            {
                Mode.PerSec   => 1f / _perSec.Value,
                Mode.Interval => _perSec.Value,
                _             => throw new ArgumentOutOfRangeException()
            };
            
            _scale.Value = (_timer / interval).Clamp01();
            
            if (_timer >= interval)
            {
                _timer -= interval;
                _onInvoke.Invoke();
            }
            
            _timer += _realTime ? Time.unscaledDeltaTime : Time.deltaTime;
        }
    }
}