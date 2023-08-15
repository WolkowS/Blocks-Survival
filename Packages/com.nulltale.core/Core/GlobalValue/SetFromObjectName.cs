using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Values
{
    public class SetFromObjectName: MonoBehaviour
    {
        public Optional<GameObject> _root;
        public bool                 _onEnable;
        public UnityEvent<string>   _onInvoke;
        
        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }

        public void Invoke()
        {
            var go = _root.GetValueOrDefault(gameObject);
            _onInvoke.Invoke(go.name);
        }
    }
}