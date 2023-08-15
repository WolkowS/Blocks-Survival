using System;
using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class SetPosition : MonoBehaviour
    {
        public Vers<GameObject>           _pos;
        public Optional<Vers<GameObject>> _target;
        public bool                       _onEnable;
        
        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }
        
        public void Invoke()
        {
            (_target.Enabled ? _target.Value.Value.transform : transform).position = _pos.Value.transform.position;
        }
    }
}