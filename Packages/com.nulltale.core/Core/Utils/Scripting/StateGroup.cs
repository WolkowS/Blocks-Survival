using System.Linq;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class StateGroup : MonoBehaviour
    {
        // =======================================================================
        public void Invoke(int index)
        {
            var states = transform.GetChildren<State>().ToArray();
            for (var n = 0; n < states.Length; n++)
            {
                var state    = states[n];
                var isActive = n == index;
                state.Invoke(isActive);
            }
        }
    }
}