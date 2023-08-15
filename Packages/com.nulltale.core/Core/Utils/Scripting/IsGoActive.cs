using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class IsGoActive : MonoBehaviour
    {
        public Vers<GameObject> _go;
        public UnityEvent       _onTrue;
        
        // =======================================================================
        public void Invoke()
        {
            var go = _go.Value;
            if (go == null || go.activeInHierarchy == false)
                return;
            
            _onTrue.Invoke();
        }
    }
}