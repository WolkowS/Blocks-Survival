using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.Values
{
    public class InitGlobalValues : MonoBehaviour
    {
        public List<GlobalValue> _values;

        // =======================================================================
        private void Awake()
        {
            foreach (var value in _values)
            {
                if (value == null)
                    continue;
                
                value.Init();
            }
        }
    }
}