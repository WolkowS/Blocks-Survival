using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreLib.Values
{
    public class GvrGoRandom : GlobalValueReadonly<GameObject>
    {
        public List<GlobalValue<GameObject>> _list;
        
        public override GameObject Value
        {
            get => _list.Where(n => n.Value != null).Random();
            set => throw new NotImplementedException();
        }
        
        // =======================================================================
        public override string ToString()
        {
            return Value.ToString();
        }
        
        internal override string Serialize()
        {
            return string.Empty;
        }

        internal override void Deserialize(string data)
        {
        }
    }
}