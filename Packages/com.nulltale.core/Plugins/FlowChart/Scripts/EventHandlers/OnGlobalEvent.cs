using CoreLib.Events;
using UnityEngine;

namespace Fungus
{
    [EventHandlerInfo("",
                      "On Event",
                      "The block will execute when the global event triggered.")]
    [AddComponentMenu("")]
    public class OnGlobalEvent : EventHandler
    {
        [SerializeField] protected bool   _once = true;
        [SerializeField] protected GeVoid _event;
        private                    bool   _fired;

        private void OnEnable()
        {
            _event.OnInvoke += _execute;
        }

        private void OnDisable()
        {
            _event.OnInvoke -= _execute;
        }

        private void _execute()
        {
            if (_once && _fired)
                return;
            
            _fired = true;
            ExecuteBlock();
        }
    }
}