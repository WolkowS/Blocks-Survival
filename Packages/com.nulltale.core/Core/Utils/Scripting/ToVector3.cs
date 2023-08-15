using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class ToVector3 : MonoBehaviour
    {
        public UnityEvent<Vector3> _onInvoke;
        
        // =======================================================================
        public void Invoke(Vector2 vec)
        {
            _onInvoke.Invoke(vec.To3DXY());
        }
    }
}