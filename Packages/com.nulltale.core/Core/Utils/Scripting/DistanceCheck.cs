using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class DistanceCheck : MonoBehaviour
    {
        public Vers<GameObject>  _go;
        public UnityEvent<float> _onInvoke;
        
        // =======================================================================
        public void Invoke()
        {
            _onInvoke.Invoke(Vector3.Distance(transform.position, _go.Value.transform.position));
        }
    }
}