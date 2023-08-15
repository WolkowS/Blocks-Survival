using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class SetScale : MonoBehaviour
    {
        public Vers<Vector3>              _scale;
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
            (_target.Enabled ? _target.Value.Value.transform : transform).localScale = _scale.Value;
        }
    }
}