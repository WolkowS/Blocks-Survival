using UnityEngine;

namespace CoreLib.Events
{
    public class InvokeGeVoid : MonoBehaviour, InvokeActivator.IHandle
    {
        public GeVoid _trigger;
        
        // =======================================================================
        public void Invoke()
        {
            _trigger.Invoke();
        }
    }
}