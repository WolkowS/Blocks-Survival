using System;
using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace CoreLib
{
    [Serializable]
    public class RefBus : MonoBehaviour
    {
        public Vers<Object> _link;

        public Object Link
        {
            get => _link.Value;
            set
            {
                if (_link.Value == value)
                    return;
                
                _link.Value = value;
            }
        }
    }
}