using System;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class RandomValueFloat : MonoBehaviour
    {
        public ParticleSystem.MinMaxCurve _output;
        public bool                       _onEnable;
        public UnityEvent<float>          _onInvoke;
        
        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }

        public void Invoke()
        {
            _onInvoke.Invoke(_output.Evaluate());
        }
    }
}