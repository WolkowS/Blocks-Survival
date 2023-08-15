using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class Invert : MonoBehaviour
    {
        public UnityEvent<bool> _onInvoke;
        
        public void Invoke(bool val)
        {
            _onInvoke.Invoke(!val);
        }
    }
}