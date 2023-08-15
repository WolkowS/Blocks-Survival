using System.Collections.Generic;
using System.Linq;
using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class IntSum : MonoBehaviour
    {
        public List<Vers<int>> m_Input;
        public UnityEvent<int> m_OnInvoke;

        // =======================================================================
        public void Invoke()
        {
            m_OnInvoke.Invoke(m_Input.Sum(n => n.Value));
        }
    }
}