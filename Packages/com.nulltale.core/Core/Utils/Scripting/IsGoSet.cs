using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class IsGoSet : MonoBehaviour
    {
        public Vers<GameObject> _go;
        public UnityEvent       _onFalse;
        
        // =======================================================================
        public void Invoke()
        {
            var go = _go.Value;
            if (go == null)
                _onFalse.Invoke();
            
        }
    }
}