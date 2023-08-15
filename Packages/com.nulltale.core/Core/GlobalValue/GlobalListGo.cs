using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreLib.Values
{
    public class GlobalListGo: GlobalList<GameObject>
    {
        public override void Init()
        {
            Value.Clear();
        }
        
        public IEnumerable<T> OfType<T>() where T : Component
        {
            return Value.Select(n => n.GetComponent<T>()).Where(n => n != null);
        }
    }
}