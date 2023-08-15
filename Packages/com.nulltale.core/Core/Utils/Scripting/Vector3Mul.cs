using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class Vector3Mul : MonoBehaviour
    {
        public float               _mul;
        public UnityEvent<Vector3> _onInvoke;
        
        // =======================================================================
        public void Invoke(Vector3 vec)
        {
            _onInvoke.Invoke(vec * _mul);
        }
    }
}