using System;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class SetVector2 : MonoBehaviour
    {
        public Optional<Vers<Vector2>> _vec;
        [HideIf(nameof(_vec))]
        public Vers<float> _x;
        [HideIf(nameof(_vec))]
        public Vers<float> _y;
        
        public bool _onEnable;

        public UnityEvent<Vector2> _onInvoke;
        
        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }

        public void Invoke()
        {
            _onInvoke.Invoke(_vec.Enabled ? _vec.Value.Value : new Vector2(_x.Value, _y.Value));
        }
    }
}