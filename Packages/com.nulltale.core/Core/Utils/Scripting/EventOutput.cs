using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class EventOutput : MonoBehaviour
    {
        public MonoBehaviour _target;
        public string        _event;
        
        public  UnityEvent _onInvoke;
        private Delegate   _delegate;
        
        // =======================================================================
        private void OnEnable()
        {
            var link = _target.GetType().GetEvent(_event, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //_delegate = Delegate.CreateDelegate(link.EventHandlerType, null, Invoke)
            
            //Action act = Invoke;
            link.AddEventHandler(_target, (Action)Invoke);
        }

        private void OnDisable()
        {
            var link = _target.GetType().GetEvent(_event, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            link.RemoveEventHandler(_target, (Action)Invoke);
        }
        
        public void Invoke()
        {
            _onInvoke.Invoke();
        }
    }
}