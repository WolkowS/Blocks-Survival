using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class ToVector2 : MonoBehaviour
    {
        public UnityEvent<Vector2> _onInvoke;
        
        // =======================================================================
        public void Invoke(Vector3 vec)
        {
            _onInvoke.Invoke(vec.To2DXY());
        }
    }
}