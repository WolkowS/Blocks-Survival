using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class RandomChanse : MonoBehaviour
    {
        [Range(0, 1)]
        public float _chanse;
        public bool       _onEnable;
        public UnityEvent _onInvoke;
        
        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }
        
        public void Invoke()
        {
            if (_chanse.Chance())
                _onInvoke.Invoke();
        }
    }
}