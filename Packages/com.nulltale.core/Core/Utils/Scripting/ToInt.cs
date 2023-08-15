using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class ToInt : MonoBehaviour
    {
        public UnityEvent<int> _onInvoke;
        
        // =======================================================================
        public void Round(float f)
        {
            _onInvoke.Invoke(f.RoundToInt());
        }
        
        public void Floor(float f)
        {
            _onInvoke.Invoke(f.FloorToInt());
        }
        
        public void Ceil(float f)
        {
            _onInvoke.Invoke(f.CeilToInt());
        }
    }
}