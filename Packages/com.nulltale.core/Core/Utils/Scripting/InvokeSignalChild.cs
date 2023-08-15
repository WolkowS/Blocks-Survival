using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace CoreLib.Scripting
{
    public class InvokeSignalChild : MonoBehaviour
    {
        public SignalAsset _key;
        
        // =======================================================================
        public void Invoke(int index)
        {
            var onSignal = transform.GetChild(index)
                                    .GetComponent<OnSignal>();
            
            onSignal.React(_key);
        }
    }
}