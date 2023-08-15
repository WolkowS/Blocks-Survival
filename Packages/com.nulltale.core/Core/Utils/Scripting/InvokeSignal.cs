using System;
using System.Data;
using CoreLib.Values;
using UnityEngine;
using UnityEngine.Timeline;

namespace CoreLib.Scripting
{
    public class InvokeSignal : MonoBehaviour
    {
        public SignalAsset      _key;
        public Vers<GameObject> _target;
        
        public bool _onEnable;
        
        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable == false)
                return;
            
            Invoke();
        }

        public void Invoke()
        {
            Invoke(_target.Value);
        }
        
        public void Invoke(GameObject root)
        {
            foreach (var trigger in root.GetComponentsInChildren<OnSignal>())
                trigger.React(_key);
        }
    }
}