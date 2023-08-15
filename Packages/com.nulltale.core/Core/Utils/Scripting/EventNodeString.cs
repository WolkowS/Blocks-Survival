using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class EventNodeString : MonoBehaviour
    {
        public UnityEvent<string> _onInvoke;
        
        // =======================================================================
        public void Invoke(string str)
        {
            _onInvoke.Invoke(str);
        }
    }
}